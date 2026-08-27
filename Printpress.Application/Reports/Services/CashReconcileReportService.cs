using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashReconcileReportService(
    IUnitOfWork _unitOfWork,
    ILocalizationService _loc) : ICashReconcileReportService
{
    private const decimal Tolerance = 0.0001m;

    public async Task<CashReconcileReportDto> GetReportAsync(
        Guid? cashAccountId,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        var periodStart = StartOfDayUtc(dateFrom);
        var periodEndExclusive = StartOfNextDayUtc(dateTo);

        if (periodStart is not null && periodEndExclusive is not null && periodStart >= periodEndExclusive)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.InvalidDateRange));

        var accounts = (await _unitOfWork.CashAccountRepository.AllAsync()).ToList();
        if (cashAccountId is Guid accountId)
        {
            accounts = accounts.Where(a => a.Id == accountId).ToList();
            if (accounts.Count == 0)
                throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));
        }

        var txs = (await _unitOfWork.CashTransactionRepository.FilterAsync(
            t => cashAccountId == null || t.CashAccountId == cashAccountId)).ToList();

        var txsByAccount = txs
            .GroupBy(t => t.CashAccountId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rows = accounts.Select(account =>
        {
            var accountTxs = txsByAccount.GetValueOrDefault(account.Id) ?? [];
            var computed = Net(accountTxs);

            var opening = periodStart is null
                ? 0
                : Net(accountTxs.Where(t => t.TransactionDate < periodStart));

            var periodTxs = accountTxs.Where(t =>
                (periodStart == null || t.TransactionDate >= periodStart)
                && (periodEndExclusive == null || t.TransactionDate < periodEndExclusive));

            var periodIn = periodTxs.Where(t => t.Type == CashTransactionType.In).Sum(t => t.Amount);
            var periodOut = periodTxs.Where(t => t.Type == CashTransactionType.Out).Sum(t => t.Amount);
            var periodClosing = opening + periodIn - periodOut;

            var ledgerAtPeriodEnd = periodEndExclusive is null
                ? computed
                : Net(accountTxs.Where(t => t.TransactionDate < periodEndExclusive));

            var difference = account.Balance - computed;

            return new CashReconcileAccountDto
            {
                CashAccountId = account.Id,
                CashAccountName = account.Name,
                AccountType = account.Type.ToString(),
                StoredBalance = account.Balance,
                ComputedBalance = computed,
                Difference = difference,
                IsMatched = IsZero(difference),
                OpeningBalance = opening,
                PeriodIn = periodIn,
                PeriodOut = periodOut,
                PeriodClosing = periodClosing,
                PeriodIdentityOk = IsZero(periodClosing - ledgerAtPeriodEnd)
            };
        }).OrderBy(a => a.IsMatched).ThenBy(a => a.CashAccountName).ToList();

        return new CashReconcileReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            AccountCount = rows.Count,
            MismatchCount = rows.Count(r => !r.IsMatched),
            TotalStoredBalance = rows.Sum(r => r.StoredBalance),
            TotalComputedBalance = rows.Sum(r => r.ComputedBalance),
            TotalDifference = rows.Sum(r => r.Difference),
            Accounts = rows
        };
    }

    private static decimal Net(IEnumerable<CashTransaction> txs) =>
        txs.Sum(t => t.Type == CashTransactionType.In ? t.Amount : -t.Amount);

    private static bool IsZero(decimal value) => Math.Abs(value) < Tolerance;

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
