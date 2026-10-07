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

        var dateQuery = BuildDateQuery(dateFrom, dateTo);
        var outRows = (await _unitOfWork.ReportRepository.GetConsumptionOutRowsAsync(inventoryItemId, dateFrom, dateTo))
            .Select(r => new ConsumptionOutRowDto
            {
                Id = r.Id,
                OccurredAt = r.CreatedAt,
                Cartons = r.Cartons,
                Units = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(r.Cartons, unitsPerCarton),
                WorkerName = r.WorkerName,
                Notes = r.Notes,
                ReferenceLabel = "حركة المخزن",
                ReferenceRoute = $"/inventory/transactions?itemId={inventoryItemId}&type=Out{dateQuery}"
            })
            .ToList();

        var executeRows = (await _unitOfWork.ReportRepository.GetConsumptionExecuteRowsAsync(inventoryItemId, dateFrom, dateTo))
            .Select(r => new ConsumptionExecuteRowDto
            {
                Id = r.Id,
                OccurredAt = r.ExecutionDate,
                OrderName = r.OrderName,
                WorkerName = r.WorkerName,
                Quantity = r.Quantity,
                NumberOfPages = r.NumberOfPages,
                NumberOfPrintingFaces = r.NumberOfPrintingFaces,
                IsCover = r.IsCover,
                PaperUnits = OrderInventoryItemsCalculator.CalculatePaperUsedForItem(new OrderItemUsageProjection
                {
                    Quantity = r.Quantity,
                    NumberOfPages = r.NumberOfPages,
                    NumberOfPrintingFaces = r.NumberOfPrintingFaces,
                    IsCover = r.IsCover
                }),
                Notes = r.Notes,
                ReferenceLabel = r.OrderName,
                ReferenceRoute = $"/order/groups/{r.OrderGroupId}/items/{r.OrderItemId}/history"
            })
            .ToList();

        var sellingRows = (await _unitOfWork.ReportRepository.GetDeliveredSellingRowsAsync(inventoryItemId, dateFrom, dateTo))
            .Select(r => new ConsumptionSellingRowDto
            {
                Id = r.Id,
                OccurredAt = r.DeliveryDate,
                OrderName = r.OrderName,
                Cartons = r.Cartons,
                Units = OrderInventoryItemsCalculator.CalculateUnitsFromCartons(r.Cartons, unitsPerCarton),
                Notes = r.Name,
                ReferenceLabel = r.OrderName,
                ReferenceRoute = $"/order/view/{r.OrderId}"
            })
            .ToList();

        var conversionRows = (await _unitOfWork.ReportRepository.GetConversionRowsAsync(inventoryItemId, dateFrom, dateTo))
            .Select(r => new ConsumptionConversionRowDto
            {
                Id = r.Id,
                OccurredAt = r.OccurredAt,
                Quantity = r.Quantity,
                Notes = r.Notes,
                ReferenceLabel = "تحويل إلى منتج",
                ReferenceRoute = $"/inventory/usage-conversions?itemId={inventoryItemId}{dateQuery}"
            })
            .ToList();

        var settlementRows = (await _unitOfWork.ReportRepository.GetSettlementRowsAsync(inventoryItemId, dateFrom, dateTo))
            .Select(r => new ConsumptionSettlementRowDto
            {
                Id = r.Id,
                OccurredAt = r.OccurredAt,
                Quantity = r.Quantity,
                SettlementType = r.SettlementType,
                Notes = r.Notes,
                ReferenceLabel = "تسويات الاستهلاك",
                ReferenceRoute = $"/inventory/usage-settlements?itemId={inventoryItemId}{dateQuery}"
            })
            .ToList();

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
            PeriodNetUnits = unitsIn - unitsOut,
            OutRows = outRows,
            ExecuteRows = executeRows,
            SellingRows = sellingRows,
            ConversionRows = conversionRows,
            SettlementRows = settlementRows
        };
    }

    private static string BuildDateQuery(DateTime? dateFrom, DateTime? dateToExclusive)
    {
        var parts = new List<string>();
        if (dateFrom is not null)
            parts.Add($"dateFrom={dateFrom.Value:yyyy-MM-dd}");
        if (dateToExclusive is not null)
            parts.Add($"dateTo={dateToExclusive.Value.AddDays(-1):yyyy-MM-dd}");
        return parts.Count == 0 ? "" : "&" + string.Join("&", parts);
    }
}
