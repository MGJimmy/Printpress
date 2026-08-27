namespace Printpress.Application;

internal sealed class InventoryStockOutReportService(IUnitOfWork unitOfWork) : IInventoryStockOutReportService
{
    public async Task<InventoryStockOutReportDto> GetReportAsync(
        int? categoryId, Guid? inventoryItemId, Guid? workerId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var rows = await unitOfWork.ReportRepository.GetInventoryStockOutsAsync(
            categoryId, inventoryItemId, workerId, dateFrom, dateToExclusive);

        return new InventoryStockOutReportDto
        {
            Rows = rows,
            MovementCount = rows.Count,
            TotalCartons = rows.Sum(r => r.Quantity),
            ItemCount = rows.Select(r => r.ItemId).Distinct().Count(),
            WorkerCount = rows.Where(r => r.WorkerId.HasValue).Select(r => r.WorkerId!.Value).Distinct().Count()
        };
    }
}
