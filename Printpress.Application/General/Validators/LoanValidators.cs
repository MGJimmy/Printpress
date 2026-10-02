using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class LenderUpsertDtoValidator : AbstractValidator<LenderUpsertDto>
{
    public LenderUpsertDtoValidator(ILocalizationService loc)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(loc.Get(LocalizationKeys.Lenders.NameRequired))
            .MaximumLength(200).WithMessage(loc.Get(LocalizationKeys.Lenders.NameMaxLength));

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .WithMessage(loc.Get(LocalizationKeys.Shared.MaxLength, loc.Get(LocalizationKeys.Lenders.FieldPhone), 50))
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage(loc.Get(LocalizationKeys.Shared.MaxLength, loc.Get(LocalizationKeys.Lenders.FieldNotes), 500))
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

internal sealed class LoanCreateDtoValidator : AbstractValidator<LoanCreateDto>
{
    public LoanCreateDtoValidator(ILocalizationService loc)
    {
        RuleFor(x => x.LenderId)
            .NotEmpty().WithMessage(loc.Get(LocalizationKeys.Loans.LenderRequired));

        RuleFor(x => x.Principal)
            .GreaterThan(0).WithMessage(loc.Get(LocalizationKeys.Loans.PrincipalMustBePositive));

        RuleFor(x => x.OccurredAt)
            .NotEmpty().WithMessage(loc.Get(LocalizationKeys.Loans.DateRequired));

        RuleFor(x => x.CashAccountId)
            .NotEmpty().WithMessage(loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.Loans.FieldCashAccount)));

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage(loc.Get(LocalizationKeys.Shared.MaxLength, loc.Get(LocalizationKeys.Loans.FieldNotes), 500))
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

internal sealed class LoanPayDtoValidator : AbstractValidator<LoanPayDto>
{
    public LoanPayDtoValidator(ILocalizationService loc)
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(loc.Get(LocalizationKeys.Loans.PaymentAmountInvalid));

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .WithMessage(loc.Get(LocalizationKeys.Shared.MaxLength, loc.Get(LocalizationKeys.Loans.FieldNotes), 500))
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}
