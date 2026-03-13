using FluentValidation;

namespace Printpress.Application;

internal sealed class AddSalaryTransactionDtoValidator : AbstractValidator<AddSalaryTransactionDto>
{
    public AddSalaryTransactionDtoValidator()
    {
        RuleFor(x => x.WorkerId)
            .NotEmpty().WithMessage("معرف العامل مطلوب");

        RuleFor(x => x.PayrollPeriodId)
            .NotEmpty().WithMessage("معرف دورة الراتب مطلوب");

        RuleFor(x => x.TransactionType)
            .IsInEnum().WithMessage("نوع الحركة غير صالح");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("المبلغ يجب أن يكون أكبر من صفر");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("تاريخ الحركة مطلوب");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("الملاحظة يجب ألا تتجاوز 500 حرف");
    }
}
