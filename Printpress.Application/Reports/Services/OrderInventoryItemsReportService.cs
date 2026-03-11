namespace Printpress.Application;

internal sealed class OrderInventoryItemsReportService(IUnitOfWork _unitOfWork) : IOrderInventoryItemsReportService
{
    public async Task<OrderInventoryItemsReportDto> GetReportAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        var item = await _unitOfWork.ReportRepository.GetInventoryItemDataAsync(inventoryItemId)
            ?? throw new ValidationExeption("عنصر المخزون غير موجود");

        var cartonsIn = await _unitOfWork.ReportRepository.GetInventoryCartonsInAsync(inventoryItemId, dateFrom, dateTo);
        var cartonsOut = await _unitOfWork.ReportRepository.GetInventorycartonsOutAsync(inventoryItemId, dateFrom, dateTo);
        var orderItemsUsage = await _unitOfWork.ReportRepository.GetOrderItemsUsageAsync(inventoryItemId, dateFrom, dateTo);

        var unitsPerCarton = OrderInventoryItemsCalculator.CalculateUnitsPerCarton(item.PacksPerCarton, item.UnitsPerPack);
        var unitsIn = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(cartonsIn, unitsPerCarton);
        var unitsOut = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(cartonsOut, unitsPerCarton);
        var paperUsed = OrderInventoryItemsCalculator.CalculatePaperUsed(orderItemsUsage);
        var expectedWaste = OrderInventoryItemsCalculator.CalculateExpectedWaste(paperUsed, item.ExpectedProductionWastePercent);
        var difference = OrderInventoryItemsCalculator.CalculateDifference(unitsOut, paperUsed, expectedWaste);

        return new OrderInventoryItemsReportDto
        {
            ItemCategory = item.CategoryName,
            ItemName = item.Name,
            PacksPerCarton = item.PacksPerCarton,
            UnitsPerPack = item.UnitsPerPack,
            CartonsIn = cartonsIn,
            UnitsIn =  unitsIn,
            CartonsOut = cartonsOut,
            UnitsOut = unitsOut,
            PaperUsedUnits = paperUsed,
            ExpectedWaste = expectedWaste,
            Difference = difference
        };
    }

    public Task<List<InventoryCategoryFilterDto>> GetCategoriesAsync()
        => _unitOfWork.ReportRepository.GetInventoryCategoriesForReportAsync();

    public Task<List<InventoryItemFilterDto>> GetItemsByCategoryAsync(int categoryId)
        => _unitOfWork.ReportRepository.GetInventoryItemsForReportAsync(categoryId);
}
