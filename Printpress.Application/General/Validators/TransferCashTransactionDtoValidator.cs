using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class TransferCashTransactionDtoValidator : AbstractValidator<TransferCashTransactionDto>
{
    public TransferCashTransactionDtoValidator(ILocalizationService loc)
    {
        RuleFor(x => x.FromCashAccountId)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldFromAccount)));

        RuleFor(x => x.ToCashAccountId)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldToAccount)));

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.MustBePositive, loc.Get(LocalizationKeys.CashAccounts.FieldAmount)));

        RuleFor(x => x.TransactionDate)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.CashAccounts.FieldDate)));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.MaxLength, loc.Get(LocalizationKeys.CashAccounts.FieldDescription), 500));
    }
}
