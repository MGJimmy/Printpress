using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashFlowReportService(
    IUnitOfWork _unitOfWork,
    ILocalizationService _loc) : ICashFlowReportService
{
    public async Task<CashFlowReportDto> GetReportAsync(
        Guid? cashAccountId,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        var periodStart = StartOfDayUtc(dateFrom);
        var periodEndExclusive = StartOfNextDayUtc(dateTo);

        if (periodStart is not null && periodEndExclusive is not null && periodStart >= periodEndExclusive)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.InvalidDateRange));

        if (cashAccountId is Guid accountId)
        {
            _ = await _unitOfWork.CashAccountRepository.FindAsync(accountId)
                ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));
        }

        var txs = (await _unitOfWork.CashTransactionRepository.FilterAsync(
            t => (cashAccountId == null || t.CashAccountId == cashAccountId)
                 && (periodStart == null || t.TransactionDate >= periodStart)
                 && (periodEndExclusive == null || t.TransactionDate < periodEndExclusive))).ToList();

        var byDay = txs
            .GroupBy(t => t.TransactionDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => Bucket(
                g.Key.ToString("yyyy-MM-dd"),
                g.Key.ToString("yyyy-MM-dd"),
                g,
                g.Key,
                g.Key))
            .ToList();

        var byMonth = txs
            .GroupBy(t => new DateTime(t.TransactionDate.Year, t.TransactionDate.Month, 1))
            .OrderBy(g => g.Key)
            .Select(g => Bucket(
                g.Key.ToString("yyyy-MM"),
                g.Key.ToString("yyyy-MM"),
                g,
                g.Key,
                g.Key.AddMonths(1).AddDays(-1)))
            .ToList();

        return new CashFlowReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            CashAccountId = cashAccountId,
            TotalIn = txs.Where(t => t.Type == CashTransactionType.In).Sum(t => t.Amount),
            TotalOut = txs.Where(t => t.Type == CashTransactionType.Out).Sum(t => t.Amount),
            Net = txs.Sum(t => t.Type == CashTransactionType.In ? t.Amount : -t.Amount),
            ByDay = byDay,
            ByMonth = byMonth
        };
    }

    private static CashFlowBucketDto Bucket(
        string key,
        string label,
        IEnumerable<CashTransaction> items,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var list = items.ToList();
        var totalIn = list.Where(t => t.Type == CashTransactionType.In).Sum(t => t.Amount);
        var totalOut = list.Where(t => t.Type == CashTransactionType.Out).Sum(t => t.Amount);
        return new CashFlowBucketDto
        {
            Key = key,
            Label = label,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            TotalIn = totalIn,
            TotalOut = totalOut,
            Net = totalIn - totalOut,
            TransactionCount = list.Count
        };
    }

    private static DateTime? StartOfDayUtc(DateTime? value) =>
        value is null ? null : DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Utc);

    private static DateTime? StartOfNextDayUtc(DateTime? value) =>
        value is null ? null : DateTime.SpecifyKind(value.Value.Date.AddDays(1), DateTimeKind.Utc);
}
