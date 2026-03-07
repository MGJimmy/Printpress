using FluentValidation;

namespace Printpress.Application;

public class PurchaseInvoiceLineCreateDtoValidator : AbstractValidator<PurchaseInvoiceLineCreateDto>
{
    public PurchaseInvoiceLineCreateDtoValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(PurchaseInvoiceLineCreateDto.InventoryItemId)));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(ResponseMessage.MustBePositive(nameof(PurchaseInvoiceLineCreateDto.Quantity)));

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage(ResponseMessage.MustBePositive(nameof(PurchaseInvoiceLineCreateDto.UnitPrice)));
    }
}
