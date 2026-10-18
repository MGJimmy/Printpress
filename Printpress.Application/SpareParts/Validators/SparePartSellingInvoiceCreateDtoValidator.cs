using FluentValidation;

namespace Printpress.Application;

public class SparePartSellingInvoiceCreateDtoValidator : AbstractValidator<SparePartSellingInvoiceCreateDto>
{
    public SparePartSellingInvoiceCreateDtoValidator()
    {
        RuleFor(x => x.ClientName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.InvoiceDate).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Notes));
        RuleFor(x => x.Lines).NotEmpty().WithMessage("يجب إضافة سطر واحد على الأقل");
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.SparePartItemId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0);
            line.RuleFor(x => x.UnitPrice).GreaterThan(0);
        });
    }
}
