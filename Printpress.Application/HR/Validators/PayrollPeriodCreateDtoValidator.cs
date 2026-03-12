using FluentValidation;

namespace Printpress.Application;

public class PayrollPeriodCreateDtoValidator : AbstractValidator<PayrollPeriodCreateDto>
{
    public PayrollPeriodCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم دورة الرواتب مطلوب")
            .MaximumLength(200).WithMessage("اسم دورة الرواتب يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البداية مطلوب");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاريخ النهاية مطلوب")
            .GreaterThan(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
    }
}
