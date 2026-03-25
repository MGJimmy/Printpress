using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class AddCashTransactionDtoValidator : AbstractValidator<AddCashTransactionDto>
{
    public AddCashTransactionDtoValidator()
    {
        RuleFor(x => x.CashAccountId)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(AddCashTransactionDto.CashAccountId)));

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(AddCashTransactionDto.Type)))
            .Must(v => EnumHelper.IsValidEnumValue(typeof(CashTransactionType), v))
            .WithMessage(ResponseMessage.CreateInvalidEnumValueMessage(typeof(CashTransactionType), nameof(AddCashTransactionDto.Type)));

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(AddCashTransactionDto.Category)))
            .Must(v => EnumHelper.IsValidEnumValue(typeof(CashTransactionCategory), v))
            .WithMessage(ResponseMessage.CreateInvalidEnumValueMessage(typeof(CashTransactionCategory), nameof(AddCashTransactionDto.Category)));

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage(ResponseMessage.MustBePositive(nameof(AddCashTransactionDto.Amount)));

        RuleFor(x => x.TransactionDate)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(AddCashTransactionDto.TransactionDate)));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(ResponseMessage.MaxLength(nameof(AddCashTransactionDto.Description), 500));

        When(x => !string.IsNullOrEmpty(x.ReferenceType), () =>
        {
            RuleFor(x => x.ReferenceType)
                .Must(v => EnumHelper.IsValidEnumValue(typeof(CashTransactionReferenceType), v))
                .WithMessage(ResponseMessage.CreateInvalidEnumValueMessage(typeof(CashTransactionReferenceType), nameof(AddCashTransactionDto.ReferenceType)));

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithMessage(ResponseMessage.Required(nameof(AddCashTransactionDto.ReferenceId)));
        });
    }
}
