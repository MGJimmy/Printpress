using Printpress.Domain;

namespace Printpress.Application;

internal static class InvoiceSettlementHelper
{
    public static decimal ResolvePaidNow(decimal? paidNow, decimal total, ILocalizationService loc)
    {
        var paid = paidNow ?? total;
        if (paid < 0)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Invoices.PaymentAmountInvalid));
        if (paid > total)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Invoices.PaymentExceedsRemaining));
        return paid;
    }

    public static string BuildPaymentDescription(ILocalizationService loc, string descriptionKey, string invoiceNumber, string note)
    {
        var description = loc.Get(descriptionKey, invoiceNumber);
        var trimmedNote = note?.Trim();
        if (!string.IsNullOrWhiteSpace(trimmedNote))
            description = $"{description} ({trimmedNote})";
        if (description.Length > 500)
            description = description[..500];
        return description;
    }

    public static async Task AddCashOutAsync(
        IUnitOfWork unitOfWork,
        CashAccountDomainService cashAccountDomainService,
        ILocalizationService loc,
        CashAccountType accountType,
        CashTransactionReferenceType referenceType,
        Guid invoiceId,
        decimal amount,
        string description,
        DateTime transactionDate)
    {
        if (amount <= 0)
            return;

        var cashAccount = await unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == accountType)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.NotFound));

        cashAccountDomainService.AddCashAccountTransaction(
            cashAccount,
            CashTransactionType.Out,
            CashTransactionCategory.Purchases,
            referenceType,
            invoiceId,
            amount,
            description,
            transactionDate);

        unitOfWork.CashAccountRepository.Update(cashAccount);
    }

    public static async Task<List<InvoicePaymentDto>> GetPaymentsAsync(
        IUnitOfWork unitOfWork,
        CashTransactionReferenceType referenceType,
        Guid invoiceId)
    {
        var txs = await unitOfWork.CashTransactionRepository.FilterAsync(
            t => t.ReferenceType == referenceType
                 && t.ReferenceId == invoiceId
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
