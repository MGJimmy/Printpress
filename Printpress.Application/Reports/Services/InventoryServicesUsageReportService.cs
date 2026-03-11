namespace Printpress.Application;

internal sealed class InventoryServicesUsageReportService(IUnitOfWork _unitOfWork)
    : IInventoryServicesUsageReportService
{
    public async Task<InventoryServicesUsageReportDto> GetReportAsync(
        int inventoryItemCategoryId, Guid serviceCategoryId, DateTime? dateFrom, DateTime? dateTo)
    {
        var inventoryItems = await _unitOfWork.ReportRepository
            .GetInventoryItemsStockByCategoryAsync(inventoryItemCategoryId, dateFrom, dateTo);

        var services = await _unitOfWork.ReportRepository
            .GetServicesByCategoryIdAsync(serviceCategoryId);

        var serviceIds = services.Select(s => s.Id).ToList();

        var orderCounts = serviceIds.Count > 0
            ? await _unitOfWork.ReportRepository.GetOrderCountsByServiceAsync(serviceIds, dateFrom, dateTo)
            : new Dictionary<Guid, int>();

        var itemRawData = serviceIds.Count > 0
            ? await _unitOfWork.ReportRepository.GetServiceItemRawDataAsync(serviceIds, dateFrom, dateTo)
            : new List<ServiceItemRaw>();

        var inventoryRows = BuildInventoryRows(inventoryItems);
        var serviceRows = BuildServiceRows(services, orderCounts, itemRawData);

        return new InventoryServicesUsageReportDto
        {
            InventoryItems = inventoryRows,
            TotalCartonsIn = inventoryRows.Sum(r => r.CartonsIn),
            TotalUnitsIn = inventoryRows.Sum(r => r.UnitsIn),
            TotalCartonsOut = inventoryRows.Sum(r => r.CartonsOut),
            TotalUnitsOut = inventoryRows.Sum(r => r.UnitsOut),
            Services = serviceRows,
            TotalOrders = serviceRows.Sum(r => r.OrderCount),
            TotalItems = serviceRows.Sum(r => r.ItemCount),
            TotalPaperUsed = serviceRows.Sum(r => r.PaperUsed)
        };
    }

    public Task<List<ServiceCategoryFilterDto>> GetServiceCategoriesAsync()
        => _unitOfWork.ReportRepository.GetAllServiceCategoriesAsync();

    // ── Inventory rows ───────────────────────────────────────────────────────

    private static List<InventoryItemUsageRowDto> BuildInventoryRows(
        List<InventoryItemStockProjection> items)
    {
        return items.Select(i => new InventoryItemUsageRowDto
        {
            ItemCategory = i.CategoryName,
            ItemName = i.Name,
            PacksPerCarton = i.PacksPerCarton,
            UnitsPerPack = i.UnitsPerPack,
            CartonsIn = i.CartonsIn,
            UnitsIn = CalculateUnits(i.CartonsIn, i.PacksPerCarton, i.UnitsPerPack),
            CartonsOut = i.CartonsOut,
            UnitsOut = CalculateUnits(i.CartonsOut, i.PacksPerCarton, i.UnitsPerPack),
            ExpectedProductionWastePercent = i.ExpectedProductionWastePercent
        }).ToList();
    }

    private static int CalculateUnits(int cartons, int? packsPerCarton, int? unitsPerPack)
    {
        if (packsPerCarton is null or 0 || unitsPerPack is null or 0) return cartons;
        return cartons * packsPerCarton.Value * unitsPerPack.Value;
    }

    // ── Service rows ─────────────────────────────────────────────────────────

    private static List<ServiceUsageRowDto> BuildServiceRows(
        List<ServiceBasicInfo> services,
        Dictionary<Guid, int> orderCounts,
        List<ServiceItemRaw> itemRawData)
    {
        var itemsByService = itemRawData
            .GroupBy(r => r.ServiceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return services.Select(s =>
        {
            var rawItems = itemsByService.GetValueOrDefault(s.Id, []);
            return new ServiceUsageRowDto
            {
                ServiceName = s.Name,
                OrderCount = orderCounts.GetValueOrDefault(s.Id, 0),
                ItemCount = rawItems.Count,
                PaperUsed = CalculateTotalPaperUsed(rawItems)
            };
        }).ToList();
    }

    private static decimal CalculateTotalPaperUsed(List<ServiceItemRaw> items)
        => items.Sum(CalculatePaperUsedForItem);

    private static decimal CalculatePaperUsedForItem(ServiceItemRaw item)
    {
        int pages = int.TryParse(item.PagesValue, out var p) ? p : 0;
        int faces = int.TryParse(item.FacesValue, out var f) && f > 0 ? f : 1;
        return Math.Round((decimal)(item.Quantity * pages) / faces, 2);
    }
}
