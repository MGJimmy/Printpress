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
            .ThenInclude(os => os.Service) // ÅÐÇ ÊÍÊÇÌ ÈíÇäÇÊ ÇáÎÏãÉ
        .Include(o => o.OrderGroups)
            .ThenInclude(og => og.OrderGroupServices)
                .ThenInclude(ogs => ogs.Service) // ÅÐÇ ÊÍÊÇÌ ÈíÇäÇÊ ÇáÎÏãÉ ÇáãÑÊÈØÉ ÈÇáÜ group service
        .Include(o => o.OrderGroups)
            .ThenInclude(og => og.Items)
                .ThenInclude(i => i.Details)
        .Where(o => o.Services.Any(os => serviceIds.Contains(os.ServiceId)))
        .ToListAsync();


        List<OrderItem> orderItems = orders.SelectMany(o => o.OrderGroups)
            .Where(og => og.OrderGroupServices.Any(os => serviceIds.Contains(os.ServiceId)))
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

    public async Task<List<InventoryCategoryFilterDto>> GetInventoryCategoriesForReportAsync()
    {
        return await _context.ServiceCategory
            .Where(sc => sc.InventoryItemCategoryId != null && sc.InventoryItemCategory_LKP != null)
            .GroupBy(sc => sc.InventoryItemCategoryId!.Value)
            .Select(g => new InventoryCategoryFilterDto
            {
                Id = g.Key,
                Name = g.First().InventoryItemCategory_LKP.Name
            })
            .ToListAsync();
    }

    public async Task<List<InventoryItemFilterDto>> GetInventoryItemsForReportAsync(int categoryId)
    {
        var linkedItemIds = await _context.Service
            .Where(s => s.InventoryItemId != null)
            .Select(s => s.InventoryItemId!.Value)
            .Distinct()
            .ToListAsync();

        return await _context.InventoryItem
            .Where(i => i.InventoryItemCategoryId == categoryId && linkedItemIds.Contains(i.Id))
            .Select(i => new InventoryItemFilterDto { Id = i.Id, Name = i.Name })
            .ToListAsync();
    }
}
