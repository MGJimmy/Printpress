using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashByDocumentReportService(
    IUnitOfWork _unitOfWork,
    ILocalizationService _loc) : ICashByDocumentReportService
{
    public async Task<CashByDocumentReportDto> GetReportAsync(
        Guid? cashAccountId,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        var periodStart = dateFrom is null ? (DateTime?)null : DateTime.SpecifyKind(dateFrom.Value.Date, DateTimeKind.Utc);
        var periodEndExclusive = dateTo is null ? (DateTime?)null : DateTime.SpecifyKind(dateTo.Value.Date.AddDays(1), DateTimeKind.Utc);

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

        var documents = txs
            .GroupBy(t => new { t.ReferenceType, t.ReferenceId })
            .Select(g =>
            {
                var totalIn = g.Where(t => t.Type == CashTransactionType.In).Sum(t => t.Amount);
                var totalOut = g.Where(t => t.Type == CashTransactionType.Out).Sum(t => t.Amount);
                return new CashDocumentGroupDto
                {
                    ReferenceType = g.Key.ReferenceType,
                    ReferenceId = g.Key.ReferenceId,
                    ReferenceTypeName = g.Key.ReferenceType?.ToString() ?? "None",
                    TransactionCount = g.Count(),
                    TotalIn = totalIn,
                    TotalOut = totalOut,
                    Net = totalIn - totalOut
                };
            })
            .OrderBy(d => d.ReferenceTypeName)
            .ThenByDescending(d => d.TotalIn + d.TotalOut)
            .ToList();

        return new CashByDocumentReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            CashAccountId = cashAccountId,
            TotalIn = txs.Where(t => t.Type == CashTransactionType.In).Sum(t => t.Amount),
            TotalOut = txs.Where(t => t.Type == CashTransactionType.Out).Sum(t => t.Amount),
            Documents = documents
        };
    }
}
