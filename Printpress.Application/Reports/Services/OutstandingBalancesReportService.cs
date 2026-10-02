using Printpress.Domain;

namespace Printpress.Application;

internal sealed class OutstandingBalancesReportService(
    IUnitOfWork unitOfWork,
    IWorkerTransactionCalculator workerTransactionCalculator) : IOutstandingBalancesReportService
{
    public async Task<OutstandingBalancesReportDto> GetReportAsync()
    {
        var repo = unitOfWork.ReportRepository;

        var orders = await repo.GetOpenOrdersAsync();
        var advances = await repo.GetUnpaidWorkerAdvancesAsync();
        var purchases = await repo.GetOpenPurchaseInvoicesAsync();
        var sparePurchases = await repo.GetOpenSparePartPurchaseInvoicesAsync();
        var loans = await repo.GetOpenLoansAsync();

        var rows = new List<OutstandingBalanceRowDto>();
        rows.AddRange(orders.Select(p => Map(OutstandingBalanceDirection.Collect, OutstandingBalanceKind.Order, p, $"/order/view/{p.Id}")));
        rows.AddRange(advances.Select(p => Map(OutstandingBalanceDirection.Collect, OutstandingBalanceKind.WorkerAdvance, p, $"/hr/workers/{p.Id}")));
        rows.AddRange(purchases.Select(p => Map(OutstandingBalanceDirection.Pay, OutstandingBalanceKind.PurchaseInvoice, p, $"/inventory/stock-in/invoices/{p.Id}")));
        rows.AddRange(sparePurchases.Select(p => Map(OutstandingBalanceDirection.Pay, OutstandingBalanceKind.SparePartPurchaseInvoice, p, $"/spare-parts/stock-in/invoices/{p.Id}")));
        rows.AddRange(loans.Select(p => Map(OutstandingBalanceDirection.Pay, OutstandingBalanceKind.Loan, p, $"/general/loans/{p.Id}")));
        rows.AddRange(await MapMonthlySalaryRowsAsync(repo));

        rows = rows
            .OrderBy(r => r.Direction)
            .ThenByDescending(r => r.Date)
            .ToList();

        return new OutstandingBalancesReportDto
        {
            TotalToCollect = rows.Where(r => r.Direction == OutstandingBalanceDirection.Collect).Sum(r => r.Remaining),
            TotalToPay = rows.Where(r => r.Direction == OutstandingBalanceDirection.Pay).Sum(r => r.Remaining),
            Rows = rows
        };
    }

    private async Task<List<OutstandingBalanceRowDto>> MapMonthlySalaryRowsAsync(IReportRepository repo)
    {
        var workers = await repo.GetActiveMonthlySalaryWorkersAsync();
        if (workers.Count == 0)
            return [];

        var now = DateTime.UtcNow;
        var firstOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var txs = await repo.GetSalaryTransactionsFromAsync(firstOfMonth, workers.Select(w => w.Id).ToList());
        var txsByWorker = txs.GroupBy(t => t.WorkerId).ToDictionary(g => g.Key, g => g.ToList());

        var rows = new List<OutstandingBalanceRowDto>();
        foreach (var workerRow in workers)
        {
            var worker = new Worker
            {
                Id = workerRow.Id,
                Name = workerRow.Name,
                SalaryType = SalaryType.Monthly,
                MonthlySalary = workerRow.MonthlySalary
            };

            var monthTxs = txsByWorker.TryGetValue(workerRow.Id, out var list)
                ? list.Select(t => new WorkerSalaryTransaction
                {
                    WorkerId = t.WorkerId,
                    TransactionType = t.TransactionType,
                    Amount = t.Amount
                })
                : [];

            var summary = workerTransactionCalculator.Calculate(worker, monthTxs);
            if (summary.RemainingThisMonth is not > 0)
                continue;

            rows.Add(new OutstandingBalanceRowDto
            {
                Direction = OutstandingBalanceDirection.Pay,
                Type = OutstandingBalanceKind.MonthlySalary,
                PartyName = workerRow.Name,
                DocumentLabel = firstOfMonth.ToString("yyyy-MM"),
                Date = firstOfMonth,
                Total = workerRow.MonthlySalary,
                Paid = summary.TotalPaidThisMonth,
                Remaining = summary.RemainingThisMonth.Value,
                Route = $"/hr/workers/{workerRow.Id}"
            });
        }

        return rows;
    }

    private static OutstandingBalanceRowDto Map(
        OutstandingBalanceDirection direction,
        OutstandingBalanceKind type,
        OutstandingDocumentProjection projection,
        string route)
    {
        return new OutstandingBalanceRowDto
        {
            Direction = direction,
            Type = type,
            PartyName = projection.PartyName,
            DocumentLabel = projection.DocumentLabel,
            Date = projection.Date,
            Total = projection.Total,
            Paid = projection.Paid,
            Remaining = projection.Remaining,
            Route = route
        };
    }
}
