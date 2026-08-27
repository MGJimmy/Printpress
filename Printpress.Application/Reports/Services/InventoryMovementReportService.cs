using Printpress.Domain;

namespace Printpress.Application;

internal sealed class InventoryMovementReportService(IUnitOfWork unitOfWork) : IInventoryMovementReportService
{
    public async Task<InventoryMovementReportDto> GetReportAsync(
        Guid inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var item = await unitOfWork.ReportRepository.GetInventoryItemDataAsync(inventoryItemId)
            ?? throw new ValidationExeption("عنصر المخزون غير موجود");

        var transactions = await unitOfWork.ReportRepository.GetInventoryMovementsAsync(inventoryItemId);

        var opening = transactions
            .Where(t => dateFrom != null && t.CreatedAt < dateFrom)
            .Sum(SignedQuantity);

        var period = transactions
            .Where(t => (dateFrom == null || t.CreatedAt >= dateFrom)
                && (dateToExclusive == null || t.CreatedAt < dateToExclusive))
            .OrderBy(t => t.CreatedAt)
            .ThenBy(t => t.Id)
            .ToList();

        var running = opening;
        var lines = new List<InventoryMovementLineDto>(period.Count);
        foreach (var t in period)
        {
            var inQty = t.Type == InventoryTransactionType.In ? t.Quantity : 0;
            var outQty = t.Type == InventoryTransactionType.In ? 0 : t.Quantity;
            running += inQty - outQty;
            lines.Add(new InventoryMovementLineDto
            {
                Id = t.Id,
                MovementDate = t.CreatedAt,
                Type = TypeLabel(t.Type),
                InQuantity = inQty,
                OutQuantity = outQty,
                RunningBalance = running,
                ReferenceType = ReferenceLabel(t.ReferenceType),
                WorkerName = t.WorkerName,
                Notes = t.Notes
            });
        }

        return new InventoryMovementReportDto
        {
            ItemId = inventoryItemId,
            ItemName = item.Name,
            CategoryName = item.CategoryName,
            OpeningBalance = opening,
            TotalIn = period.Where(t => t.Type == InventoryTransactionType.In).Sum(t => t.Quantity),
            TotalOut = period.Where(t => t.Type != InventoryTransactionType.In).Sum(t => t.Quantity),
            ClosingBalance = running,
            Lines = lines
        };
    }

    private static int SignedQuantity(InventoryMovementTxProjection t)
        => t.Type == InventoryTransactionType.In ? t.Quantity : -t.Quantity;

    private static string TypeLabel(InventoryTransactionType type) => type switch
    {
        InventoryTransactionType.In => "دخول",
        InventoryTransactionType.Out => "خروج",
        InventoryTransactionType.Adjustment => "تسوية",
        _ => type.ToString()
    };

    private static string ReferenceLabel(InventoryTransactionReferenceType type) => type switch
    {
        InventoryTransactionReferenceType.Purchase => "فاتورة شراء",
        InventoryTransactionReferenceType.StockAdjustment => "صرف يدوي",
        InventoryTransactionReferenceType.Order => "طلب",
        _ => type.ToString()
    };
}
