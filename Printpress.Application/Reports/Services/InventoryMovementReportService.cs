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
            .Where(t => dateFrom != null && t.OccurredAt < dateFrom)
            .Sum(SignedQuantity);

        var period = transactions
            .Where(t => (dateFrom == null || t.OccurredAt >= dateFrom)
                && (dateToExclusive == null || t.OccurredAt < dateToExclusive))
            .OrderBy(t => t.OccurredAt)
            .ThenBy(t => t.Id)
            .ToList();

        var purchaseLines = await LoadPurchaseLinesAsync(period);
        var dateQuery = BuildDateQuery(dateFrom, dateToExclusive);

        var running = opening;
        var lines = new List<InventoryMovementLineDto>(period.Count);
        foreach (var t in period)
        {
            var inQty = t.Type == InventoryTransactionType.In ? t.Quantity : 0;
            var outQty = t.Type == InventoryTransactionType.In ? 0 : t.Quantity;
            running += inQty - outQty;
            var (label, route) = ResolveReference(t, inventoryItemId, purchaseLines, dateQuery);
            lines.Add(new InventoryMovementLineDto
            {
                Id = t.Id,
                MovementDate = t.OccurredAt,
                Type = TypeLabel(t.Type),
                InQuantity = inQty,
                OutQuantity = outQty,
                RunningBalance = running,
                ReferenceType = TypeLabel(t.ReferenceType),
                ReferenceLabel = label,
                ReferenceRoute = route,
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

    private async Task<Dictionary<Guid, PurchaseInvoiceLine>> LoadPurchaseLinesAsync(
        List<InventoryMovementTxProjection> period)
    {
        var purchaseIds = period
            .Where(t => t.ReferenceType == InventoryTransactionReferenceType.Purchase)
            .Select(t => t.ReferenceId)
            .Distinct()
            .ToList();

        if (purchaseIds.Count == 0)
            return [];

        var lines = await unitOfWork.PurchaseInvoiceLineRepository.FilterAsync(
            l => purchaseIds.Contains(l.Id),
            nameof(PurchaseInvoiceLine.PurchaseInvoice));

        return lines.ToDictionary(l => l.Id);
    }

    private static (string Label, string Route) ResolveReference(
        InventoryMovementTxProjection transaction,
        Guid inventoryItemId,
        Dictionary<Guid, PurchaseInvoiceLine> purchaseLines,
        string dateQuery)
    {
        return transaction.ReferenceType switch
        {
            InventoryTransactionReferenceType.Purchase when purchaseLines.TryGetValue(transaction.ReferenceId, out var line)
                => (
                    $"فاتورة شراء: {(line.PurchaseInvoice is null ? "—" : line.PurchaseInvoice.InvoiceNumber.ToString())}",
                    $"/inventory/stock-in/invoices/{line.PurchaseInvoiceId}"),
            InventoryTransactionReferenceType.Purchase
                => ("فاتورة شراء", ""),
            InventoryTransactionReferenceType.Order
                => ("طلب", $"/order/view/{transaction.ReferenceId}"),
            InventoryTransactionReferenceType.StockAdjustment
                => ("صرف يدوي", $"/inventory/transactions?itemId={inventoryItemId}&type=Out{dateQuery}"),
            _ => ("—", "")
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

    private static string TypeLabel(InventoryTransactionReferenceType type) => type switch
    {
        InventoryTransactionReferenceType.Purchase => "فاتورة شراء",
        InventoryTransactionReferenceType.StockAdjustment => "صرف يدوي",
        InventoryTransactionReferenceType.Order => "طلب",
        _ => type.ToString()
    };

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
