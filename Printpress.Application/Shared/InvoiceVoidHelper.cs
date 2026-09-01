using Printpress.Domain;

namespace Printpress.Application;

internal static class InvoiceVoidHelper
{
    public static string RequireReason(string reason, ILocalizationService loc)
    {
        var trimmed = reason?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ValidationExeption(loc.Get(LocalizationKeys.Invoices.ReasonRequired));
        if (trimmed.Length > 500)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Shared.MaxLength, loc.Get(LocalizationKeys.Invoices.Reason), 500));
        return trimmed;
    }

    public static async Task<string> ResolveUserNameAsync(IUserDisplayNameService users, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId) || users is null)
            return userId;
        return await users.GetDisplayNameAsync(userId);
    }

    public static void EnsureDateRange(DateTime? from, DateTime? toExclusive, ILocalizationService loc)
    {
        if (from is not null && toExclusive is not null && from >= toExclusive)
            throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.InvalidDateRange));
    }

    public static async Task VoidLinkedCashAsync(
        IUnitOfWork unitOfWork,
        CashAccountDomainService cashAccountDomainService,
        ILocalizationService loc,
        CashTransactionReferenceType referenceType,
        Guid invoiceId,
        string reason)
    {
        var original = await unitOfWork.CashTransactionRepository.FirstOrDefaultAsync(
            t => t.ReferenceType == referenceType
                 && t.ReferenceId == invoiceId
                 && !t.IsVoided
                 && t.ReversesTransactionId == null)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.TransactionNotFound));

        var account = await unitOfWork.CashAccountRepository.FindAsync(original.CashAccountId)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.NotFound));

        var description = loc.Get(LocalizationKeys.CashAccounts.VoidDescription, original.Description ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(reason))
            description = $"{description} ({reason})";
        if (description.Length > 500)
            description = description[..500];

        cashAccountDomainService.Void(account, original, description, DateTime.UtcNow);
        unitOfWork.CashTransactionRepository.Update(original);
        unitOfWork.CashAccountRepository.Update(account);
    }
}
