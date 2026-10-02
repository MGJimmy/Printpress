namespace Printpress.Domain;

public class Loan : Entity
{
    public Guid LenderId { get; private set; }
    public int LoanNumber { get; private set; }
    public decimal Principal { get; private set; }
    public decimal PaidAmount { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string Notes { get; private set; }
    public Guid CashAccountId { get; private set; }
    public bool IsVoided { get; private set; }
    public string VoidReason { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string VoidedBy { get; private set; }

    public virtual Lender Lender { get; private set; }

    public decimal Remaining => Principal - PaidAmount;
    public bool IsClosed => !IsVoided && Remaining == 0;

    private Loan()
    {
    }

    public Loan(Guid lenderId, decimal principal, DateTime occurredAt, Guid cashAccountId, string notes)
    {
        if (lenderId == Guid.Empty)
            throw new BusinessExceptions(LocalizationKeys.Loans.LenderRequired);
        if (cashAccountId == Guid.Empty)
            throw new BusinessExceptions(LocalizationKeys.Loans.CashAccountRequired);
        if (principal <= 0)
            throw new BusinessExceptions(LocalizationKeys.Loans.PrincipalMustBePositive);
        if (occurredAt == default)
            throw new BusinessExceptions(LocalizationKeys.Loans.DateRequired);

        LenderId = lenderId;
        Principal = principal;
        PaidAmount = 0;
        OccurredAt = occurredAt;
        CashAccountId = cashAccountId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public void ApplyPayment(decimal amount)
    {
        if (IsVoided)
            throw new BusinessExceptions(LocalizationKeys.Loans.AlreadyVoided);
        if (amount <= 0)
            throw new BusinessExceptions(LocalizationKeys.Loans.PaymentAmountInvalid);
        if (amount > Remaining)
            throw new BusinessExceptions(LocalizationKeys.Loans.PaymentExceedsRemaining);

        PaidAmount += amount;
    }

    public void ReversePayment(decimal amount)
    {
        if (amount <= 0)
            throw new BusinessExceptions(LocalizationKeys.Loans.PaymentAmountInvalid);
        if (amount > PaidAmount)
            throw new BusinessExceptions(LocalizationKeys.Loans.ReverseExceedsPaid);

        PaidAmount -= amount;
    }

    public void MarkAsVoided(string reason, string userId)
    {
        if (IsVoided)
            throw new BusinessExceptions(LocalizationKeys.Loans.AlreadyVoided);
        if (PaidAmount > 0)
            throw new BusinessExceptions(LocalizationKeys.Loans.CannotVoidHasPayments);

        IsVoided = true;
        VoidReason = reason;
        VoidedAt = DateTime.UtcNow;
        VoidedBy = userId;
    }
}
