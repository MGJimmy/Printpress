using Microsoft.EntityFrameworkCore;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.Infrastructure;

internal class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItemReportData?> GetInventoryItemDataAsync(Guid inventoryItemId)
    {
        return await _context.InventoryItem
            .Where(i => i.Id == inventoryItemId)
            .Select(i => new InventoryItemReportData
            {
                Name = i.Name,
                CategoryName = i.InventoryItemCategory_LKP.Name,
                PacksPerCarton = i.PacksPerCarton,
                UnitsPerPack = i.UnitsPerPack,
                ExpectedProductionWastePercent = i.ExpectedProductionWastePercent
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetInventoryCartonsInAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        return await _context.InventoryTransaction
            .Where(t => t.InventoryItemId == inventoryItemId
                && t.InventoryTransactionType == InventoryTransactionType.In
                && (dateFrom == null || t.CreatedAt >= dateFrom)
                && (dateTo == null || t.CreatedAt < dateTo))
            .SumAsync(t => (int?)t.Quantity) ?? 0;
    }

    public async Task<int> GetInventorycartonsOutAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        return await _context.InventoryTransaction
            .Where(t => t.InventoryItemId == inventoryItemId
                && t.InventoryTransactionType == InventoryTransactionType.Out
                && (dateFrom == null || t.CreatedAt >= dateFrom)
                && (dateTo == null || t.CreatedAt < dateTo))
            .SumAsync(t => (int?)t.Quantity) ?? 0;
    }

    public async Task<List<OrderItemUsageProjection>> GetOrderItemsUsageAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        var serviceIds = await _context.Service
            .Where(s => s.InventoryItemId == inventoryItemId)
            .Select(s => s.Id)
            .ToListAsync();

        if (!serviceIds.Any())
            return [];

        var orders = await _context.Order
            .Include(o => o.OrderGroups)
                .ThenInclude(og => og.OrderGroupServices)
                    .ThenInclude(ogs => ogs.Service)
            .Include(o => o.OrderGroups)
                .ThenInclude(og => og.Items)
                    .ThenInclude(i => i.Details)
            .Where(o => o.Services.Any(os => serviceIds.Contains(os.ServiceId)))
            .ToListAsync();

        var candidates = orders
            .SelectMany(o => o.OrderGroups ?? [])
            .Where(og => og.ExecutionType != GroupExecutionType.External_Full)
            .SelectMany(og => (og.OrderGroupServices ?? [])
                .Where(os => serviceIds.Contains(os.ServiceId) && os.Service != null)
                .SelectMany(os => (og.Items ?? [])
                    .Where(item => !item.IsDeleted)
                    .Select(item => (Item: item, GroupService: os))))
            .ToList();

        if (candidates.Count == 0)
            return [];

        var executedQuantities = await GetExecutedQuantitiesAsync(
            candidates.Select(c => c.Item.Id), dateFrom, dateTo);

        return candidates
            .Select(c =>
            {
                var categoryId = c.GroupService.Service.ServiceCategoryId;
                if (!executedQuantities.TryGetValue((c.Item.Id, categoryId), out var executedQty) || executedQty <= 0)
                    return null;

                return new OrderItemUsageProjection
                {
                    Quantity = executedQty,
                    NumberOfPages = int.TryParse(c.Item.Details?.FirstOrDefault(d => d.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages)?.Value, out var pages) ? pages : 0,
                    NumberOfPrintingFaces = int.TryParse(c.Item.Details?.FirstOrDefault(d => d.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces)?.Value, out var faces) ? faces : 0,
                    IsCover = c.GroupService.IsCover
                };
            })
            .Where(p => p != null)
            .Select(p => p!)
            .ToList();
    }


    public async Task<List<OrderItemUsageProjection>> GetOrderItemsUsageAsync_old(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        // Get service IDs linked to this inventory item
        var serviceIds = await _context.Service
            .Where(s => s.InventoryItemId == inventoryItemId)
            .Select(s => s.Id)
            .ToListAsync();

        if (!serviceIds.Any())
            return [];

        // Get order group IDs that use those services within the date range
        var orderGroupIds = await _context.OrderGroupService
            .Where(ogs => serviceIds.Contains(ogs.ServiceId)
                && !ogs.IsDeleted
                && !ogs.OrderGroup.IsDeleted
                && !ogs.OrderGroup.Order.IsDeleted
                && (dateFrom == null || ogs.OrderGroup.Order.CreatedAt >= dateFrom)
                && (dateTo == null || ogs.OrderGroup.Order.CreatedAt <= dateTo))
            .Select(ogs => ogs.OrderGroupId)
            .Distinct()
            .ToListAsync();

        if (!orderGroupIds.Any())
            return [];

        // Load raw data from DB
        var rawItems = await _context.Item
            .Where(item => orderGroupIds.Contains(item.OrderGroupId) && !item.IsDeleted)
            .Select(item => new
            {
                item.Quantity,
                PagesValue = item.Details
                    .Where(d => d.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages && !d.IsDeleted)
                    .Select(d => d.Value).FirstOrDefault(),
                FacesValue = item.Details
                    .Where(d => d.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces && !d.IsDeleted)
                    .Select(d => d.Value).FirstOrDefault()
            })
            .ToListAsync();

        // Process in memory
        return rawItems
            .Select(r => new OrderItemUsageProjection
            {
                Quantity = r.Quantity,
                NumberOfPages = int.TryParse(r.PagesValue, out var pages) ? pages : 0,
                NumberOfPrintingFaces = int.TryParse(r.FacesValue, out var faces) ? faces : 0
            })
            .Where(p => p.NumberOfPrintingFaces > 0)
            .ToList();
    }

    // ── Report 2: Inventory & Services Usage ────────────────────────────────

    public async Task<List<InventoryItemStockProjection>> GetInventoryItemsStockByCategoryAsync(
        int categoryId, DateTime? dateFrom, DateTime? dateTo)
    {
        return await _context.InventoryItem
            .Where(i => i.InventoryItemCategoryId == categoryId)
            .Select(i => new InventoryItemStockProjection
            {
                Id = i.Id,
                Name = i.Name,
                CategoryName = i.InventoryItemCategory_LKP.Name,
                PacksPerCarton = i.PacksPerCarton,
                UnitsPerPack = i.UnitsPerPack,
                ExpectedProductionWastePercent = i.ExpectedProductionWastePercent,
                CartonsIn = i.InventoryTransactions
                    .Where(t => t.InventoryTransactionType == InventoryTransactionType.In
                        && (dateFrom == null || t.CreatedAt >= dateFrom)
                        && (dateTo == null || t.CreatedAt < dateTo))
                    .Sum(t => (int?)t.Quantity) ?? 0,
                CartonsOut = i.InventoryTransactions
                    .Where(t => t.InventoryTransactionType == InventoryTransactionType.Out
                        && (dateFrom == null || t.CreatedAt >= dateFrom)
                        && (dateTo == null || t.CreatedAt < dateTo))
                    .Sum(t => (int?)t.Quantity) ?? 0,
                CurrentStockCartons =
                    (i.InventoryTransactions.Where(t => t.InventoryTransactionType == InventoryTransactionType.In).Sum(t => (int?)t.Quantity) ?? 0)
                    - (i.InventoryTransactions.Where(t => t.InventoryTransactionType == InventoryTransactionType.Out).Sum(t => (int?)t.Quantity) ?? 0)
            })
            .ToListAsync();
    }

    public async Task<List<ServiceCategoryFilterDto>> GetAllServiceCategoriesAsync()
    {
        return await _context.ServiceCategory
            .Select(sc => new ServiceCategoryFilterDto { Id = sc.Id, Name = sc.Name })
            .ToListAsync();
    }

    public async Task<List<ServiceBasicInfo>> GetServicesByCategoryIdAsync(Guid serviceCategoryId)
    {
        return await _context.Service
            .Where(s => s.ServiceCategoryId == serviceCategoryId)
            .Select(s => new ServiceBasicInfo { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetOrderCountsByServiceAsync(
        List<Guid> serviceIds, DateTime? dateFrom, DateTime? dateTo)
    {
        return await _context.OrderService
            .Where(os => serviceIds.Contains(os.ServiceId)
                && !os.IsDeleted
                && !os.Order.IsDeleted
                && (dateFrom == null || os.Order.CreatedAt >= dateFrom)
                && (dateTo == null || os.Order.CreatedAt < dateTo))
            .GroupBy(os => os.ServiceId)
            .Select(g => new
            {
                ServiceId = g.Key,
                OrderCount = g.Select(os => os.OrderId).Distinct().Count()
            })
            .ToDictionaryAsync(x => x.ServiceId, x => x.OrderCount);
    }

    public async Task<List<ServiceItemRaw>> GetServiceItemRawDataAsync(
        List<Guid> serviceIds, DateTime? dateFrom, DateTime? dateTo)
    {
        var serviceGroupPairs = await _context.OrderGroupService
            .Where(ogs => serviceIds.Contains(ogs.ServiceId)
                && !ogs.IsDeleted
                && !ogs.OrderGroup.IsDeleted
                && !ogs.OrderGroup.Order.IsDeleted)
            .Select(ogs => new
            {
                ogs.ServiceId,
                ogs.OrderGroupId,
                ogs.IsCover,
                ogs.Service.ServiceCategoryId
            })
            .Distinct()
            .ToListAsync();

        if (!serviceGroupPairs.Any()) return [];

        var groupIds = serviceGroupPairs.Select(p => p.OrderGroupId).Distinct().ToList();

        var rawItems = await _context.Item
            .Where(i => groupIds.Contains(i.OrderGroupId) && !i.IsDeleted && i.OrderGroup.ExecutionType != GroupExecutionType.External_Full)
            .Select(i => new
            {
                i.Id,
                i.OrderGroupId,
                PagesValue = i.Details
                    .Where(d => d.ItemDetailsKeyId == (int)ItemDetailsKeyEnum.NumberOfPages && !d.IsDeleted)
                    .Select(d => d.Value).FirstOrDefault(),
                FacesValue = i.Details
                    .Where(d => d.ItemDetailsKeyId == (int)ItemDetailsKeyEnum.NumberOfPrintingFaces && !d.IsDeleted)
                    .Select(d => d.Value).FirstOrDefault()
            })
            .ToListAsync();

        if (rawItems.Count == 0)
            return [];

        var executedQuantities = await GetExecutedQuantitiesAsync(
            rawItems.Select(i => i.Id), dateFrom, dateTo);

        return rawItems
            .Join(serviceGroupPairs,
                item => item.OrderGroupId,
                pair => pair.OrderGroupId,
                (item, pair) => new { item, pair })
            .Select(x =>
            {
                if (!executedQuantities.TryGetValue((x.item.Id, x.pair.ServiceCategoryId), out var executedQty) || executedQty <= 0)
                    return null;

                return new ServiceItemRaw
                {
                    ServiceId = x.pair.ServiceId,
                    Quantity = executedQty,
                    PagesValue = x.item.PagesValue,
                    FacesValue = x.item.FacesValue,
                    IsCover = x.pair.IsCover
                };
            })
            .Where(r => r != null)
            .Select(r => r!)
            .ToList();
    }

    private async Task<Dictionary<(Guid OrderItemId, Guid ServiceCategoryId), int>> GetExecutedQuantitiesAsync(
        IEnumerable<Guid> orderItemIds, DateTime? dateFrom, DateTime? dateTo)
    {
        var itemIds = orderItemIds.Distinct().ToList();
        if (itemIds.Count == 0)
            return [];

        var rows = await _context.WorkerProduction
            .Where(e => itemIds.Contains(e.OrderItemId)
                && (dateFrom == null || e.ExecutionDate >= dateFrom)
                && (dateTo == null || e.ExecutionDate < dateTo))
            .GroupBy(e => new { e.OrderItemId, e.ServiceCategoryId })
            .Select(g => new
            {
                g.Key.OrderItemId,
                g.Key.ServiceCategoryId,
                Quantity = g.Sum(e => e.Quantity)
            })
            .ToListAsync();

        return rows
            .Where(r => r.Quantity > 0)
            .ToDictionary(r => (r.OrderItemId, r.ServiceCategoryId), r => r.Quantity);
    }
}
