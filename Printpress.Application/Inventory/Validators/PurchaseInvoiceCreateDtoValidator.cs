using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class PurchaseInvoiceCreateDtoValidator : AbstractValidator<PurchaseInvoiceCreateDto>
{

    public PurchaseInvoiceCreateDtoValidator(IGenericRepository<InventoryItem> inventoryItemRepository)
    {
        RuleFor(x => x.InvoiceNumber)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(PurchaseInvoiceCreateDto.InvoiceNumber)))
            .MaximumLength(100)
            .WithMessage(ResponseMessage.MaxLength(nameof(PurchaseInvoiceCreateDto.InvoiceNumber), 100));

        RuleFor(x => x.SupplierName)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(PurchaseInvoiceCreateDto.SupplierName)))
            .MaximumLength(300)
            .WithMessage(ResponseMessage.MaxLength(nameof(PurchaseInvoiceCreateDto.SupplierName), 300));

        RuleFor(x => x.InvoiceDate)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(PurchaseInvoiceCreateDto.InvoiceDate)));

        RuleFor(x => x.AttachmentFilePath)
            .MaximumLength(500)
            .WithMessage(ResponseMessage.MaxLength(nameof(PurchaseInvoiceCreateDto.AttachmentFilePath), 500))
            .When(x => !string.IsNullOrEmpty(x.AttachmentFilePath));

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(PurchaseInvoiceCreateDto.Lines)))
            .MustAsync(async (lines, cancellation) =>
            {
                var itemIds = lines.Select(l => l.InventoryItemId).Distinct().ToList();
                return await inventoryItemRepository.AllExistAsync(itemIds, cancellation);
            })
            .WithMessage("One or more selected inventory items do not exist in our inventory."); ;

        RuleForEach(x => x.Lines)
            .SetValidator(new PurchaseInvoiceLineCreateDtoValidator());
            
    }
}
