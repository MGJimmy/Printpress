namespace Printpress.Application;

internal sealed class InventoryStockBalanceReportService(IUnitOfWork unitOfWork)
    : IInventoryStockBalanceReportService
{
    public async Task<InventoryStockBalanceReportDto> GetReportAsync(
        int? categoryId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var rows = await unitOfWork.ReportRepository
            .GetInventoryStockBalanceAsync(categoryId, dateFrom, dateToExclusive);

        return new InventoryStockBalanceReportDto
        {
            Rows = rows,
            ItemCount = rows.Count,
            TotalOpeningCartons = rows.Sum(r => r.OpeningCartons),
            TotalPeriodInCartons = rows.Sum(r => r.PeriodInCartons),
            TotalPeriodOutCartons = rows.Sum(r => r.PeriodOutCartons),
            TotalClosingCartons = rows.Sum(r => r.ClosingCartons),
            TotalClosingUnits = rows.Sum(r => r.ClosingUnits)
        };
    }
}
