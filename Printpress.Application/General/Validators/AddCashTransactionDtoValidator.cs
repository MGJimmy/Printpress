using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class AddCashTransactionDtoValidator : AbstractValidator<AddCashTransactionDto>
{
    public AddCashTransactionDtoValidator(ILocalizationService loc)
    {
        RuleFor(x => x.CashAccountId)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldCashAccount)));

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldType)))
            .Must(v => EnumHelper.IsValidEnumValue(typeof(CashTransactionType), v))
            .WithMessage(_ => loc.Get(LocalizationKeys.CashAccounts.InvalidType));

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldCategory)))
            .Must(v => EnumHelper.IsValidEnumValue(typeof(CashTransactionCategory), v))
            .WithMessage(_ => loc.Get(LocalizationKeys.CashAccounts.InvalidCategory));

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.MustBePositive, loc.Get(LocalizationKeys.CashAccounts.FieldAmount)));

        RuleFor(x => x.TransactionDate)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldDate)));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.MaxLength, loc.Get(LocalizationKeys.CashAccounts.FieldDescription), 500));

        When(x => !string.IsNullOrEmpty(x.ReferenceType), () =>
        {
            RuleFor(x => x.ReferenceType)
                .Must(v => EnumHelper.IsValidEnumValue(typeof(CashTransactionReferenceType), v))
                .WithMessage(_ => loc.Get(LocalizationKeys.CashAccounts.InvalidReferenceType));

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldReferenceId)));
        });
    }
}
