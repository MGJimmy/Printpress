namespace Printpress.Application;

internal sealed class OrderInventoryItemsReportService(IUnitOfWork _unitOfWork) : IOrderInventoryItemsReportService
{
    public async Task<OrderInventoryItemsReportDto> GetReportAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo)
    {
        var item = await _unitOfWork.ReportRepository.GetInventoryItemDataAsync(inventoryItemId)
            ?? throw new ValidationExeption("عنصر المخزون غير موجود");

        var cartonsIn = await _unitOfWork.ReportRepository.GetInventoryCartonsInAsync(inventoryItemId, dateFrom, dateTo);
        var cartonsOut = await _unitOfWork.ReportRepository.GetInventorycartonsOutAsync(inventoryItemId, dateFrom, dateTo);
        var stockIn = await _unitOfWork.ReportRepository.GetInventoryCartonsInAsync(inventoryItemId, null, null);
        var stockOut = await _unitOfWork.ReportRepository.GetInventorycartonsOutAsync(inventoryItemId, null, null);
        var orderItemsUsage = await _unitOfWork.ReportRepository.GetOrderItemsUsageAsync(inventoryItemId, dateFrom, dateTo);
        var deliveredSellingCartons = await _unitOfWork.ReportRepository.GetDeliveredSellingCartonsAsync(inventoryItemId, dateFrom, dateTo);

        var unitsPerCarton = OrderInventoryItemsCalculator.CalculateUnitsPerCarton(item.PacksPerCarton, item.UnitsPerPack);
        var unitsIn = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(cartonsIn, unitsPerCarton);
        var unitsOut = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(cartonsOut, unitsPerCarton);
        var deliveredSellingUnits = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(deliveredSellingCartons, unitsPerCarton);
        var currentStockCartons = stockIn - stockOut;
        var paperUsed = OrderInventoryItemsCalculator.CalculatePaperUsed(orderItemsUsage);
        var consumption = OrderInventoryItemsCalculator.CalculateConsumption(paperUsed, deliveredSellingUnits);
        var conversionUnits = await _unitOfWork.ReportRepository.GetConversionUnitsAsync(inventoryItemId, dateFrom, dateTo);
        var expectedWaste = OrderInventoryItemsCalculator.CalculateExpectedWaste(paperUsed, item.ExpectedProductionWastePercent);
        var difference = OrderInventoryItemsCalculator.CalculateDifference(unitsOut, consumption, conversionUnits, expectedWaste);
        var settlementUnits = await _unitOfWork.ReportRepository.GetUsageSettlementUnitsAsync(inventoryItemId, dateFrom, dateTo);

        return new OrderInventoryItemsReportDto
        {
            ItemCategory = item.CategoryName,
            ItemName = item.Name,
            PacksPerCarton = item.PacksPerCarton,
            UnitsPerPack = item.UnitsPerPack,
            CartonsIn = cartonsIn,
            UnitsIn = unitsIn,
            CartonsOut = cartonsOut,
            UnitsOut = unitsOut,
            PaperUsedUnits = consumption,
            ConversionUnits = conversionUnits,
            ExpectedWaste = expectedWaste,
            Difference = difference,
            SettlementUnits = settlementUnits,
            UnexplainedDifference = OrderInventoryItemsCalculator.CalculateUnexplainedDifference(difference, settlementUnits),
            CurrentStockCartons = currentStockCartons,
            CurrentStockUnits = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(currentStockCartons, unitsPerCarton),
            PeriodNetCartons = cartonsIn - cartonsOut,
            PeriodNetUnits = unitsIn - unitsOut
        };
    }
}
