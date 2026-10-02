using Printpress.Domain;

namespace Printpress.Application;

internal static class LoanCashHelper
{
    public static string BuildDescription(ILocalizationService loc, string descriptionKey, object formatArg, string note)
    {
        var description = loc.Get(descriptionKey, formatArg);
        var trimmedNote = note?.Trim();
        if (!string.IsNullOrWhiteSpace(trimmedNote))
            description = $"{description} ({trimmedNote})";
        if (description.Length > 500)
            description = description[..500];
        return description;
    }

    public static async Task AddAsync(
        IUnitOfWork unitOfWork,
        CashAccountDomainService cashAccountDomainService,
        ILocalizationService loc,
        Guid cashAccountId,
        CashTransactionType type,
        Guid loanId,
        decimal amount,
        string description,
        DateTime transactionDate)
    {
        var cashAccount = await unitOfWork.CashAccountRepository.FindAsync(cashAccountId)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.NotFound));

        cashAccountDomainService.AddCashAccountTransaction(
            cashAccount,
            type,
            CashTransactionCategory.Loan,
            CashTransactionReferenceType.Loan,
            loanId,
            amount,
            description,
            transactionDate);

        unitOfWork.CashAccountRepository.Update(cashAccount);
    }

    public static async Task<List<InvoicePaymentDto>> GetRepaymentsAsync(IUnitOfWork unitOfWork, Guid loanId)
    {
        var txs = await unitOfWork.CashTransactionRepository.FilterAsync(
            t => t.ReferenceType == CashTransactionReferenceType.Loan
                 && t.ReferenceId == loanId
                 && t.Type == CashTransactionType.Out
                 && t.ReversesTransactionId == null);

        return txs
            .OrderBy(t => t.TransactionDate)
            .ThenBy(t => t.CreatedAt)
            .Select(t => new InvoicePaymentDto
            {
                Id = t.Id,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                IsVoided = t.IsVoided
            })
            .ToList();
    }
}
