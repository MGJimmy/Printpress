using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class LoanService(
    IUnitOfWork unitOfWork,
    IValidator<LoanCreateDto> createValidator,
    IValidator<LoanPayDto> payValidator,
    IGuidGenerator guidGenerator,
    CashAccountDomainService cashAccountDomainService,
    IUserDisplayNameService userDisplayNameService,
    ILocalizationService loc) : ILoanService
{
    public async Task<PagedList<LoanListDto>> GetAllAsync(
        Guid? lenderId,
        bool? hasRemaining,
        bool? isVoided,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize)
    {
        InvoiceVoidHelper.EnsureDateRange(dateFrom, dateToExclusive, loc);

        var paged = await unitOfWork.LoanRepository.FilterAsync(
            new Paging(pageNumber, pageSize),
            l => (lenderId == null || l.LenderId == lenderId)
                && (isVoided == null || l.IsVoided == isVoided)
                && (hasRemaining == null || (hasRemaining.Value
                    ? !l.IsVoided && l.PaidAmount < l.Principal
                    : l.IsVoided || l.PaidAmount >= l.Principal))
                && (dateFrom == null || l.OccurredAt >= dateFrom)
                && (dateToExclusive == null || l.OccurredAt < dateToExclusive),
            new Sorting(nameof(Loan.OccurredAt), SortingDirection.DESC),
            nameof(Loan.Lender));

        return new PagedList<LoanListDto>(
            paged.Items.Select(MapList).ToList(),
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize);
    }

    public async Task<LoanDto> GetByIdAsync(Guid id)
    {
        var loan = await LoadAsync(id, track: false);
        var dto = MapDetail(loan);
        var cashAccount = await unitOfWork.CashAccountRepository.FindAsync(loan.CashAccountId);
        dto.CashAccountName = cashAccount?.Name;
        dto.VoidedByName = await InvoiceVoidHelper.ResolveUserNameAsync(userDisplayNameService, loan.VoidedBy);
        dto.Payments = await LoanCashHelper.GetRepaymentsAsync(unitOfWork, loan.Id);
        return dto;
    }

    public async Task<LoanDto> AddAsync(LoanCreateDto payload, string userId)
    {
        if (payload is null)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Shared.InvalidPayload));

        var validation = await createValidator.ValidateAsync(payload);
        if (!validation.IsValid)
            throw new ValidationExeption(validation.Errors.First().ErrorMessage);

        var lender = await unitOfWork.LenderRepository.FindAsync(payload.LenderId)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.Lenders.NotFound));

        var cashAccount = await unitOfWork.CashAccountRepository.FindAsync(payload.CashAccountId)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.NotFound));

        var loan = new Loan(
            lender.Id,
            payload.Principal,
            UtcDateTime.AsUtc(payload.OccurredAt),
            cashAccount.Id,
            payload.Notes);
        loan.Id = guidGenerator.NewGuid();

        await unitOfWork.LoanRepository.AddAsync(loan);

        await LoanCashHelper.AddAsync(
            unitOfWork,
            cashAccountDomainService,
            loc,
            cashAccount.Id,
            CashTransactionType.In,
            loan.Id,
            loan.Principal,
            LoanCashHelper.BuildDescription(
                loc,
                LocalizationKeys.CashAccounts.LoanDisbursementDescription,
                lender.Name,
                payload.Notes),
            loan.OccurredAt);

        await unitOfWork.SaveChangesAsync(userId);

        return await GetByIdAsync(loan.Id);
    }

    public async Task PayAsync(Guid id, LoanPayDto payload, string userId)
    {
        if (payload is null)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Shared.InvalidPayload));

        var validation = await payValidator.ValidateAsync(payload);
        if (!validation.IsValid)
            throw new ValidationExeption(validation.Errors.First().ErrorMessage);

        var loan = await LoadAsync(id, track: true);
        if (loan.IsVoided)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Loans.AlreadyVoided));
        if (loan.Remaining <= 0)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Loans.AlreadyFullyPaid));

        var cashAccount = await unitOfWork.CashAccountRepository.FindAsync(loan.CashAccountId)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.NotFound));

        loan.ApplyPayment(payload.Amount);

        var occurredAt = payload.OccurredAt.HasValue
            ? UtcDateTime.AsUtc(payload.OccurredAt.Value)
            : DateTime.UtcNow;

        await LoanCashHelper.AddAsync(
            unitOfWork,
            cashAccountDomainService,
            loc,
            cashAccount.Id,
            CashTransactionType.Out,
            loan.Id,
            payload.Amount,
            LoanCashHelper.BuildDescription(
                loc,
                LocalizationKeys.CashAccounts.LoanRepaymentDescription,
                loan.LoanNumber,
                payload.Note),
            occurredAt);

        unitOfWork.LoanRepository.Update(loan);
        await unitOfWork.SaveChangesAsync(userId);
    }

    public async Task VoidAsync(Guid id, string reason, string userId)
    {
        reason = InvoiceVoidHelper.RequireReason(reason, loc);

        var loan = await LoadAsync(id, track: true);
        loan.MarkAsVoided(reason, userId);

        var originals = (await unitOfWork.CashTransactionRepository.FilterAsync(
            t => t.ReferenceType == CashTransactionReferenceType.Loan
                 && t.ReferenceId == loan.Id
                 && t.Type == CashTransactionType.In
                 && !t.IsVoided
                 && t.ReversesTransactionId == null)).ToList();

        foreach (var original in originals)
        {
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

        unitOfWork.LoanRepository.Update(loan);
        await unitOfWork.SaveChangesAsync(userId);
    }

    public async Task VoidPaymentAsync(Guid id, LoanVoidPaymentDto payload, string userId)
    {
        var reason = InvoiceVoidHelper.RequireReason(payload?.Reason, loc);

        var loan = await LoadAsync(id, track: true);

        var original = await unitOfWork.CashTransactionRepository.FindAsync(payload.PaymentId)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.Loans.PaymentNotFound));

        if (original.ReferenceType != CashTransactionReferenceType.Loan
            || original.ReferenceId != loan.Id
            || original.Type != CashTransactionType.Out
            || original.ReversesTransactionId is not null)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Loans.PaymentNotFound));

        var account = await unitOfWork.CashAccountRepository.FindAsync(original.CashAccountId)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.CashAccounts.NotFound));

        var description = loc.Get(LocalizationKeys.CashAccounts.VoidDescription, original.Description ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(reason))
            description = $"{description} ({reason})";
        if (description.Length > 500)
            description = description[..500];

        cashAccountDomainService.Void(account, original, description, DateTime.UtcNow);
        loan.ReversePayment(original.Amount);

        unitOfWork.CashTransactionRepository.Update(original);
        unitOfWork.CashAccountRepository.Update(account);
        unitOfWork.LoanRepository.Update(loan);
        await unitOfWork.SaveChangesAsync(userId);
    }

    private async Task<Loan> LoadAsync(Guid id, bool track)
    {
        return await unitOfWork.LoanRepository.FirstOrDefaultAsync(
                l => l.Id == id,
                track,
                nameof(Loan.Lender))
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.Loans.NotFound));
    }

    private static LoanListDto MapList(Loan loan) => new()
    {
        Id = loan.Id,
        LoanNumber = loan.LoanNumber,
        LenderId = loan.LenderId,
        LenderName = loan.Lender?.Name,
        Principal = loan.Principal,
        PaidAmount = loan.PaidAmount,
        Remaining = loan.Remaining,
        OccurredAt = loan.OccurredAt,
        IsVoided = loan.IsVoided,
        IsClosed = loan.IsClosed
    };

    private static LoanDto MapDetail(Loan loan)
    {
        var dto = new LoanDto();
        dto.Id = loan.Id;
        dto.LoanNumber = loan.LoanNumber;
        dto.LenderId = loan.LenderId;
        dto.LenderName = loan.Lender?.Name;
        dto.Principal = loan.Principal;
        dto.PaidAmount = loan.PaidAmount;
        dto.Remaining = loan.Remaining;
        dto.OccurredAt = loan.OccurredAt;
        dto.IsVoided = loan.IsVoided;
        dto.IsClosed = loan.IsClosed;
        dto.Notes = loan.Notes;
        dto.CashAccountId = loan.CashAccountId;
        dto.VoidReason = loan.VoidReason;
        dto.VoidedAt = loan.VoidedAt;
        return dto;
    }
}
