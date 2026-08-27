using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashTreasuryReportService(
    IUnitOfWork _unitOfWork,
    ILocalizationService _loc) : ICashTreasuryReportService
{
    public async Task<CashTreasuryReportDto> GetReportAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        var periodStart = dateFrom is null ? (DateTime?)null : DateTime.SpecifyKind(dateFrom.Value.Date, DateTimeKind.Utc);
        var periodEndExclusive = dateTo is null ? (DateTime?)null : DateTime.SpecifyKind(dateTo.Value.Date.AddDays(1), DateTimeKind.Utc);

        if (periodStart is not null && periodEndExclusive is not null && periodStart >= periodEndExclusive)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.InvalidDateRange));

        var accounts = (await _unitOfWork.CashAccountRepository.AllAsync())
            .OrderBy(a => a.Name)
            .Select(a => new CashTreasuryAccountDto
            {
                CashAccountId = a.Id,
                CashAccountName = a.Name,
                AccountType = a.Type.ToString(),
                StoredBalance = a.Balance
            })
            .ToList();

        var periodTxs = (await _unitOfWork.CashTransactionRepository.FilterAsync(
            t => (periodStart == null || t.TransactionDate >= periodStart)
                 && (periodEndExclusive == null || t.TransactionDate < periodEndExclusive),
            nameof(CashTransaction.CashAccount))).ToList();

        var largestIn = periodTxs
            .Where(t => t.Type == CashTransactionType.In)
            .OrderByDescending(t => t.Amount)
            .Take(10)
            .Select(ToMovement)
            .ToList();

        var largestOut = periodTxs
            .Where(t => t.Type == CashTransactionType.Out)
            .OrderByDescending(t => t.Amount)
            .Take(10)
            .Select(ToMovement)
            .ToList();

        var transfers = periodTxs
            .Where(t => t.Category == CashTransactionCategory.Transfer && t.ReferenceId is not null)
            .GroupBy(t => t.ReferenceId.Value)
            .Select(g =>
            {
                var from = g.FirstOrDefault(t => t.Type == CashTransactionType.Out);
                var to = g.FirstOrDefault(t => t.Type == CashTransactionType.In);
                var sample = from ?? to;
                return new CashTransferRegisterRowDto
                {
                    TransferId = g.Key,
                    TransactionDate = sample?.TransactionDate ?? default,
                    Amount = sample?.Amount ?? 0,
                    FromAccountName = from?.CashAccount?.Name ?? "—",
                    ToAccountName = to?.CashAccount?.Name ?? "—",
                    Description = sample?.Description,
                    IsComplete = from is not null && to is not null
                };
            })
            .OrderByDescending(t => t.TransactionDate)
            .ToList();

        return new CashTreasuryReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            TotalStoredBalance = accounts.Sum(a => a.StoredBalance),
            Accounts = accounts,
            LargestIn = largestIn,
            LargestOut = largestOut,
            Transfers = transfers
        };
    }

    private static CashTreasuryMovementDto ToMovement(CashTransaction t) => new()
    {
        Id = t.Id,
        TransactionDate = t.TransactionDate,
        CashAccountName = t.CashAccount?.Name ?? string.Empty,
        Amount = t.Amount,
        Category = t.Category.ToString(),
        Description = t.Description
    };
}
