using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashMovementSummaryReportService(
    IUnitOfWork _unitOfWork,
    ILocalizationService _loc) : ICashMovementSummaryReportService
{
    public async Task<CashMovementSummaryReportDto> GetReportAsync(
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
                 && (periodEndExclusive == null || t.TransactionDate < periodEndExclusive),
            nameof(CashTransaction.CashAccount))).ToList();

        var byCategory = txs
            .GroupBy(t => t.Category)
            .Select(g => ToSlice(
                key: g.Key.ToString(),
                label: g.Key.ToString(),
                items: g,
                category: g.Key,
                accountId: null))
            .OrderByDescending(s => s.TotalIn + s.TotalOut)
            .ToList();

        var byAccount = txs
            .GroupBy(t => t.CashAccountId)
            .Select(g => ToSlice(
                key: g.Key.ToString(),
                label: g.First().CashAccount?.Name ?? string.Empty,
                items: g,
                category: null,
                accountId: g.Key))
            .OrderBy(s => s.Label)
            .ToList();

        return new CashMovementSummaryReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            CashAccountId = cashAccountId,
            TotalIn = txs.Where(t => t.Type == CashTransactionType.In).Sum(t => t.Amount),
            TotalOut = txs.Where(t => t.Type == CashTransactionType.Out).Sum(t => t.Amount),
            Net = txs.Sum(t => t.Type == CashTransactionType.In ? t.Amount : -t.Amount),
            TransactionCount = txs.Count,
            ByCategory = byCategory,
            ByAccount = byAccount
        };
    }

    private static CashMovementSliceDto ToSlice(
        string key,
        string label,
        IEnumerable<CashTransaction> items,
        CashTransactionCategory? category,
        Guid? accountId)
    {
        var list = items.ToList();
        var totalIn = list.Where(t => t.Type == CashTransactionType.In).Sum(t => t.Amount);
        var totalOut = list.Where(t => t.Type == CashTransactionType.Out).Sum(t => t.Amount);
        return new CashMovementSliceDto
        {
            Key = key,
            Label = label,
            Category = category,
            CashAccountId = accountId,
            TotalIn = totalIn,
            TotalOut = totalOut,
            Net = totalIn - totalOut,
            TransactionCount = list.Count
        };
    }

    private static DateTime? StartOfDayUtc(DateTime? value)
    {
        if (value is null)
            return null;
        return DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Utc);
    }

    private static DateTime? StartOfNextDayUtc(DateTime? value)
    {
        if (value is null)
            return null;
        return DateTime.SpecifyKind(value.Value.Date.AddDays(1), DateTimeKind.Utc);
    }
}
