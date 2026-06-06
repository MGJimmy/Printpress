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
                && (dateTo == null || t.CreatedAt <= dateTo))
            .SumAsync(t => (int?)t.Quantity) ?? 0;
    }

    public async Task<int> GetInventorycartonsOutAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        return await _context.InventoryTransaction
            .Where(t => t.InventoryItemId == inventoryItemId
                && t.InventoryTransactionType == InventoryTransactionType.Out
                && (dateFrom == null || t.CreatedAt >= dateFrom)
                && (dateTo == null || t.CreatedAt <= dateTo))
            .SumAsync(t => (int?)t.Quantity) ?? 0;
    }

    public async Task<List<OrderItemUsageProjection>> GetOrderItemsUsageAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        // Get service IDs linked to this inventory item
        var serviceIds = await _context.Service
            .Where(s => s.InventoryItemId == inventoryItemId)
            .Select(s => s.Id)
            .ToListAsync();

        if (!serviceIds.Any())
            return [];

        // load all orders has any service of these
        var orders = await _context.Order
        .Include(o => o.Services)
            .ThenInclude(os => os.Service) // ��� ����� ������ ������
        .Include(o => o.OrderGroups)
            .ThenInclude(og => og.OrderGroupServices)
                .ThenInclude(ogs => ogs.Service) // ��� ����� ������ ������ �������� ���� group service
        .Include(o => o.OrderGroups)
            .ThenInclude(og => og.Items)
                .ThenInclude(i => i.Details)
        .Where(o => o.Services.Any(os => serviceIds.Contains(os.ServiceId)))
        .ToListAsync();


        List<OrderItem> orderItems = orders.SelectMany(o => o.OrderGroups)
            .Where(og => og.OrderGroupServices.Any(os => serviceIds.Contains(os.ServiceId))
                && og.ExecutionType != GroupExecutionType.External_Full)
            .SelectMany(og => og.Items)
            .ToList();

        return orderItems.Select(oi => new OrderItemUsageProjection
        {
            Quantity = oi.Quantity,
            NumberOfPages = int.Parse(oi.Details.FirstOrDefault(d => d.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages)?.Value),
            NumberOfPrintingFaces = int.Parse(oi.Details.FirstOrDefault(d => d.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces)?.Value)
        }).ToList();

      
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
                        && (dateTo == null || t.CreatedAt <= dateTo))
                    .Sum(t => (int?)t.Quantity) ?? 0,
                CartonsOut = i.InventoryTransactions
                    .Where(t => t.InventoryTransactionType == InventoryTransactionType.Out
                        && (dateFrom == null || t.CreatedAt >= dateFrom)
                        && (dateTo == null || t.CreatedAt <= dateTo))
                    .Sum(t => (int?)t.Quantity) ?? 0
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
                && (dateTo == null || os.Order.CreatedAt <= dateTo))
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
                && !ogs.OrderGroup.Order.IsDeleted
                && (dateFrom == null || ogs.OrderGroup.Order.CreatedAt >= dateFrom)
                && (dateTo == null || ogs.OrderGroup.Order.CreatedAt <= dateTo))
            .Select(ogs => new { ogs.ServiceId, ogs.OrderGroupId })
            .Distinct()
            .ToListAsync();

        if (!serviceGroupPairs.Any()) return [];

        var groupIds = serviceGroupPairs.Select(p => p.OrderGroupId).Distinct().ToList();

        var rawItems = await _context.Item
            .Where(i => groupIds.Contains(i.OrderGroupId) && !i.IsDeleted && i.OrderGroup.ExecutionType != GroupExecutionType.External_Full)
            .Select(i => new
            {
                i.OrderGroupId,
                i.Quantity,
                PagesValue = i.Details
                    .Where(d => d.ItemDetailsKeyId == (int)ItemDetailsKeyEnum.NumberOfPages && !d.IsDeleted)
                    .Select(d => d.Value).FirstOrDefault(),
                FacesValue = i.Details
                    .Where(d => d.ItemDetailsKeyId == (int)ItemDetailsKeyEnum.NumberOfPrintingFaces && !d.IsDeleted)
                    .Select(d => d.Value).FirstOrDefault()
            })
            .ToListAsync();

        return rawItems
            .Join(serviceGroupPairs,
                item => item.OrderGroupId,
                pair => pair.OrderGroupId,
                (item, pair) => new ServiceItemRaw
                {
                    ServiceId = pair.ServiceId,
                    Quantity = item.Quantity,
                    PagesValue = item.PagesValue,
                    FacesValue = item.FacesValue
                })
            .ToList();
    }
}
