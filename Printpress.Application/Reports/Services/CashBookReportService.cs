using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashBookReportService(
    IUnitOfWork _unitOfWork,
    ILocalizationService _loc) : ICashBookReportService
{
    public async Task<CashBookReportDto> GetReportAsync(
        Guid? cashAccountId,
        DateTime? dateFrom,
        DateTime? dateTo,
        string type,
        string category,
        string search,
        int page,
        int pageSize)
    {
        var periodStart = StartOfDayUtc(dateFrom);
        var periodEndExclusive = StartOfNextDayUtc(dateTo);

        if (periodStart is not null && periodEndExclusive is not null && periodStart >= periodEndExclusive)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.InvalidDateRange));

        CashAccount selectedAccount = null;
        if (cashAccountId is Guid accountId)
        {
            selectedAccount = await _unitOfWork.CashAccountRepository.FindAsync(accountId)
                ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));
        }

        CashTransactionType? typeEnum = ParseType(type);
        CashTransactionCategory? categoryEnum = ParseCategory(category);

        var periodTxs = (await _unitOfWork.CashTransactionRepository.FilterAsync(
            t => (cashAccountId == null || t.CashAccountId == cashAccountId)
                 && (periodStart == null || t.TransactionDate >= periodStart)
                 && (periodEndExclusive == null || t.TransactionDate < periodEndExclusive)
                 && (typeEnum == null || t.Type == typeEnum)
                 && (categoryEnum == null || t.Category == categoryEnum),
            nameof(CashTransaction.CashAccount))).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            periodTxs = periodTxs
                .Where(t => t.Description != null && t.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var openingByAccount = await GetOpeningBalancesAsync(cashAccountId, periodStart);

        var ordered = periodTxs
            .OrderBy(t => t.CashAccountId)
            .ThenBy(t => t.TransactionDate)
            .ThenBy(t => t.CreatedAt)
            .ThenBy(t => t.Id)
            .ToList();

        var running = new Dictionary<Guid, decimal>(openingByAccount);
        var lines = new List<CashBookLineDto>(ordered.Count);

        foreach (var tx in ordered)
        {
            if (!running.ContainsKey(tx.CashAccountId))
                running[tx.CashAccountId] = 0;

            var inAmount = tx.Type == CashTransactionType.In ? tx.Amount : 0;
            var outAmount = tx.Type == CashTransactionType.Out ? tx.Amount : 0;
            running[tx.CashAccountId] += inAmount - outAmount;

            lines.Add(new CashBookLineDto
            {
                Id = tx.Id,
                TransactionDate = tx.TransactionDate,
                CashAccountId = tx.CashAccountId,
                CashAccountName = tx.CashAccount?.Name ?? string.Empty,
                InAmount = inAmount,
                OutAmount = outAmount,
                RunningBalance = running[tx.CashAccountId],
                Category = tx.Category,
                Description = tx.Description,
                Status = ResolveStatus(tx),
                CreatedBy = tx.CreatedBy
            });
        }

        var inByAccount = ordered
            .Where(t => t.Type == CashTransactionType.In)
            .GroupBy(t => t.CashAccountId)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        var outByAccount = ordered
            .Where(t => t.Type == CashTransactionType.Out)
            .GroupBy(t => t.CashAccountId)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        var accounts = (await _unitOfWork.CashAccountRepository.AllAsync()).ToList();
        if (selectedAccount is not null)
            accounts = accounts.Where(a => a.Id == selectedAccount.Id).ToList();

        var names = accounts.ToDictionary(a => a.Id, a => a.Name);
        foreach (var tx in ordered.Where(t => t.CashAccount != null))
            names[tx.CashAccountId] = tx.CashAccount.Name;

        var accountIds = accounts.Select(a => a.Id)
            .Concat(openingByAccount.Keys)
            .Concat(ordered.Select(t => t.CashAccountId))
            .Distinct()
            .ToList();

        var summaries = accountIds.Select(id =>
        {
            var opening = openingByAccount.GetValueOrDefault(id);
            var totalIn = inByAccount.GetValueOrDefault(id);
            var totalOut = outByAccount.GetValueOrDefault(id);
            return new CashBookAccountSummaryDto
            {
                CashAccountId = id,
                CashAccountName = names.GetValueOrDefault(id) ?? string.Empty,
                OpeningBalance = opening,
                TotalIn = totalIn,
                TotalOut = totalOut,
                ClosingBalance = opening + totalIn - totalOut
            };
        }).OrderBy(s => s.CashAccountName).ToList();

        var openingTotal = summaries.Sum(s => s.OpeningBalance);
        var totalInAll = summaries.Sum(s => s.TotalIn);
        var totalOutAll = summaries.Sum(s => s.TotalOut);

        var allowedPageSizes = new[] { 5, 10, 25, 50 };
        pageSize = allowedPageSizes.Contains(pageSize) ? pageSize : 10;
        page = Math.Max(1, page);
        var totalLines = lines.Count;
        var lastPage = Math.Max(1, (int)Math.Ceiling(totalLines / (double)pageSize));
        if (page > lastPage)
            page = lastPage;

        var pagedLines = totalLines == 0
            ? lines
            : lines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new CashBookReportDto
        {
            CashAccountId = cashAccountId,
            CashAccountName = selectedAccount?.Name,
            DateFrom = dateFrom,
            DateTo = dateTo,
            OpeningBalance = openingTotal,
            TotalIn = totalInAll,
            TotalOut = totalOutAll,
            ClosingBalance = openingTotal + totalInAll - totalOutAll,
            AccountSummaries = summaries,
            Lines = pagedLines,
            TotalLineCount = totalLines,
            Page = page,
            PageSize = pageSize
        };
    }

    private async Task<Dictionary<Guid, decimal>> GetOpeningBalancesAsync(Guid? cashAccountId, DateTime? periodStart)
    {
        if (periodStart is null)
            return [];

        var before = await _unitOfWork.CashTransactionRepository.FilterAsync(
            t => (cashAccountId == null || t.CashAccountId == cashAccountId)
                 && t.TransactionDate < periodStart);

        return before
            .GroupBy(t => t.CashAccountId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Type == CashTransactionType.In ? t.Amount : -t.Amount));
    }

    private static string ResolveStatus(CashTransaction tx)
    {
        if (tx.IsVoided) return "Voided";
        if (tx.ReversesTransactionId is not null) return "Reversal";
        return "Normal";
    }

    private CashTransactionType? ParseType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return null;
        if (!EnumHelper.IsValidEnumValue(typeof(CashTransactionType), type))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.InvalidType));
        return EnumHelper.MapStringToEnum<CashTransactionType>(type);
    }

    private CashTransactionCategory? ParseCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return null;
        if (!EnumHelper.IsValidEnumValue(typeof(CashTransactionCategory), category))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.InvalidCategory));
        return EnumHelper.MapStringToEnum<CashTransactionCategory>(category);
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
