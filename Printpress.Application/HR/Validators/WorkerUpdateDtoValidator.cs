using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class WorkerUpdateDtoValidator : AbstractValidator<WorkerUpdateDto>
{
    public WorkerUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرف العامل مطلوب");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم العامل مطلوب")
            .MaximumLength(200).WithMessage("اسم العامل يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50).WithMessage("رقم الهاتف يجب ألا يتجاوز 50 حرف");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("العنوان يجب ألا يتجاوز 500 حرف");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف");

        RuleFor(x => x.SalaryType)
            .IsInEnum().WithMessage("نوع الراتب غير صالح");

        RuleFor(x => x.MonthlySalary)
            .GreaterThan(0).WithMessage("الراتب الشهري يجب أن يكون أكبر من صفر")
            .When(x => x.SalaryType == SalaryType.Monthly);

        RuleFor(x => x.DailySalary)
            .GreaterThan(0).WithMessage("الراتب اليومي يجب أن يكون أكبر من صفر")
            .When(x => x.SalaryType == SalaryType.Daily);
    }
}
