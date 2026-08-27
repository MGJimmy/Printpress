using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashReferenceResolver(IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyDictionary<Guid, CashReferenceLink>> ForTransactionsAsync(IReadOnlyList<CashTransaction> transactions)
    {
        var result = new Dictionary<Guid, CashReferenceLink>();
        if (transactions.Count == 0)
            return result;

        var lookups = await LoadLookupsAsync(
            IdsOf(transactions, CashTransactionReferenceType.Order),
            IdsOf(transactions, CashTransactionReferenceType.WorkerSalaryTransaction),
            IdsOf(transactions, CashTransactionReferenceType.PurchaseInventoryInvoice),
            IdsOf(transactions, CashTransactionReferenceType.PurchaseSparePartInvoice),
            IdsOf(transactions, CashTransactionReferenceType.SellingSparePartInvoice));

        var transferIds = IdsOf(transactions, CashTransactionReferenceType.Transfer);
        var transferLegs = transferIds.Count == 0
            ? []
            : (await unitOfWork.CashTransactionRepository.FilterAsync(
                    t => t.ReferenceType == CashTransactionReferenceType.Transfer
                         && t.ReferenceId != null
                         && transferIds.Contains(t.ReferenceId.Value),
                    nameof(CashTransaction.CashAccount)))
                .ToList();

        foreach (var tx in transactions)
            result[tx.Id] = ResolveTransaction(tx, lookups, transferLegs);

        return result;
    }

    public async Task<IReadOnlyDictionary<string, CashReferenceLink>> ForDocumentKeysAsync(
        IReadOnlyList<(CashTransactionReferenceType? Type, Guid? Id)> documents)
    {
        var lookups = await LoadLookupsAsync(
            documents.Where(d => d.Type == CashTransactionReferenceType.Order && d.Id.HasValue).Select(d => d.Id!.Value).Distinct().ToList(),
            documents.Where(d => d.Type == CashTransactionReferenceType.WorkerSalaryTransaction && d.Id.HasValue).Select(d => d.Id!.Value).Distinct().ToList(),
            documents.Where(d => d.Type == CashTransactionReferenceType.PurchaseInventoryInvoice && d.Id.HasValue).Select(d => d.Id!.Value).Distinct().ToList(),
            documents.Where(d => d.Type == CashTransactionReferenceType.PurchaseSparePartInvoice && d.Id.HasValue).Select(d => d.Id!.Value).Distinct().ToList(),
            documents.Where(d => d.Type == CashTransactionReferenceType.SellingSparePartInvoice && d.Id.HasValue).Select(d => d.Id!.Value).Distinct().ToList());

        var result = new Dictionary<string, CashReferenceLink>();
        foreach (var doc in documents)
            result[DocumentKey(doc.Type, doc.Id)] = ResolveDocument(doc.Type, doc.Id, lookups);

        return result;
    }

    public static string DocumentKey(CashTransactionReferenceType? type, Guid? id)
        => $"{type?.ToString() ?? "None"}:{id?.ToString() ?? ""}";

    private async Task<Lookups> LoadLookupsAsync(
        List<Guid> orderIds,
        List<Guid> salaryIds,
        List<Guid> purchaseIds,
        List<Guid> sparePurchaseIds,
        List<Guid> spareSellIds)
    {
        var orders = orderIds.Count == 0
            ? new Dictionary<Guid, Order>()
            : (await unitOfWork.OrderRepository.FilterAsync(o => orderIds.Contains(o.Id))).ToDictionary(o => o.Id);

        var salaries = salaryIds.Count == 0
            ? new Dictionary<Guid, WorkerSalaryTransaction>()
            : (await unitOfWork.WorkerSalaryTransactionRepository.FilterAsync(
                    s => salaryIds.Contains(s.Id), nameof(WorkerSalaryTransaction.Worker)))
                .ToDictionary(s => s.Id);

        var purchases = purchaseIds.Count == 0
            ? new Dictionary<Guid, PurchaseInvoice>()
            : (await unitOfWork.PurchaseInvoiceRepository.FilterAsync(p => purchaseIds.Contains(p.Id))).ToDictionary(p => p.Id);

        var sparePurchases = sparePurchaseIds.Count == 0
            ? new Dictionary<Guid, SparePartPurchaseInvoice>()
            : (await unitOfWork.SparePartPurchaseInvoiceRepository.FilterAsync(p => sparePurchaseIds.Contains(p.Id))).ToDictionary(p => p.Id);

        var spareSells = spareSellIds.Count == 0
            ? new Dictionary<Guid, SparePartSellingInvoice>()
            : (await unitOfWork.SparePartSellingInvoiceRepository.FilterAsync(p => spareSellIds.Contains(p.Id))).ToDictionary(p => p.Id);

        return new Lookups(orders, salaries, purchases, sparePurchases, spareSells);
    }

    private static List<Guid> IdsOf(IReadOnlyList<CashTransaction> txs, CashTransactionReferenceType type)
        => txs.Where(t => t.ReferenceType == type && t.ReferenceId.HasValue)
            .Select(t => t.ReferenceId!.Value)
            .Distinct()
            .ToList();

    private static CashReferenceLink ResolveTransaction(CashTransaction tx, Lookups lookups, List<CashTransaction> transferLegs)
    {
        if (tx.ReferenceType == CashTransactionReferenceType.Transfer)
            return ResolveTransfer(tx, transferLegs);

        return ResolveDocument(tx.ReferenceType, tx.ReferenceId, lookups);
    }

    private static CashReferenceLink ResolveDocument(CashTransactionReferenceType? type, Guid? id, Lookups lookups)
    {
        if (type is null || id is null)
            return new CashReferenceLink { Label = "—" };

        var key = id.Value;
        return type switch
        {
            CashTransactionReferenceType.Order when lookups.Orders.TryGetValue(key, out var order)
                => new CashReferenceLink { Label = $"طلب: {order.Name}", Route = $"/order/view/{order.Id}" },
            CashTransactionReferenceType.Order
                => new CashReferenceLink { Label = "طلب", Route = $"/order/view/{key}" },

            CashTransactionReferenceType.WorkerSalaryTransaction when lookups.Salaries.TryGetValue(key, out var salary)
                => new CashReferenceLink
                {
                    Label = $"عامل: {salary.Worker?.Name ?? "—"}",
                    Route = $"/hr/workers/{salary.WorkerId}"
                },
            CashTransactionReferenceType.WorkerSalaryTransaction
                => new CashReferenceLink { Label = "حركة راتب" },

            CashTransactionReferenceType.PurchaseInventoryInvoice when lookups.Purchases.TryGetValue(key, out var inv)
                => new CashReferenceLink { Label = $"فاتورة مخزن: {inv.InvoiceNumber}" },
            CashTransactionReferenceType.PurchaseInventoryInvoice
                => new CashReferenceLink { Label = "فاتورة مشتريات مخزن" },

            CashTransactionReferenceType.PurchaseSparePartInvoice when lookups.SparePurchases.TryGetValue(key, out var inv)
                => new CashReferenceLink
                {
                    Label = $"شراء قطع: {inv.InvoiceNumber}",
                    Route = $"/spare-parts/stock-in/invoices?invoiceId={inv.Id}"
                },
            CashTransactionReferenceType.PurchaseSparePartInvoice
                => new CashReferenceLink { Label = "فاتورة شراء قطع غيار" },

            CashTransactionReferenceType.SellingSparePartInvoice when lookups.SpareSells.TryGetValue(key, out var inv)
                => new CashReferenceLink
                {
                    Label = $"بيع قطع: {inv.InvoiceNumber} — {inv.ClientName}",
                    Route = $"/spare-parts/stock-out/invoices?invoiceId={inv.Id}"
                },
            CashTransactionReferenceType.SellingSparePartInvoice
                => new CashReferenceLink { Label = "فاتورة بيع قطع غيار" },

            CashTransactionReferenceType.Transfer
                => new CashReferenceLink { Label = "تحويل" },

            _ => new CashReferenceLink { Label = "—" }
        };
    }

    private static CashReferenceLink ResolveTransfer(CashTransaction tx, List<CashTransaction> transferLegs)
    {
        var other = transferLegs.FirstOrDefault(t =>
            t.ReferenceId == tx.ReferenceId && t.CashAccountId != tx.CashAccountId);

        if (other?.CashAccount is not null)
        {
            var direction = tx.Type == CashTransactionType.Out ? "إلى" : "من";
            return new CashReferenceLink
            {
                Label = $"تحويل {direction} {other.CashAccount.Name}",
                Route = $"/general/cash-accounts/view/{other.CashAccountId}"
            };
        }

        return new CashReferenceLink { Label = "تحويل" };
    }

    private sealed record Lookups(
        Dictionary<Guid, Order> Orders,
        Dictionary<Guid, WorkerSalaryTransaction> Salaries,
        Dictionary<Guid, PurchaseInvoice> Purchases,
        Dictionary<Guid, SparePartPurchaseInvoice> SparePurchases,
        Dictionary<Guid, SparePartSellingInvoice> SpareSells);
}
