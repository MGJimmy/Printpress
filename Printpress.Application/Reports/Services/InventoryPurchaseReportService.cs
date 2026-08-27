namespace Printpress.Application;

internal sealed class InventoryPurchaseReportService(IUnitOfWork unitOfWork) : IInventoryPurchaseReportService
{
    public async Task<InventoryPurchaseReportDto> GetReportAsync(
        int? categoryId, Guid? inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var lines = await unitOfWork.ReportRepository.GetInventoryPurchasesAsync(
            categoryId, inventoryItemId, dateFrom, dateToExclusive);

        return new InventoryPurchaseReportDto
        {
            Lines = lines,
            InvoiceCount = lines.Select(l => l.InvoiceId).Distinct().Count(),
            LineCount = lines.Count,
            TotalQuantity = lines.Sum(l => l.Quantity),
            TotalAmount = lines.Sum(l => l.LineTotal)
        };
    }
}
