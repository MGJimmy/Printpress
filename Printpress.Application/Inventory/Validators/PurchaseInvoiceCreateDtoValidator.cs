using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class PurchaseInvoiceCreateDtoValidator : AbstractValidator<PurchaseInvoiceCreateDto>
{

    public PurchaseInvoiceCreateDtoValidator(IInventoryItemRepository inventoryItemRepository)
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
            .WithMessage(ResponseMessage.Required(nameof(PurchaseInvoiceCreateDto.Lines)));

        RuleFor(x => x.Lines)
            .MustAsync(async (lines, cancellation) =>
            {
                var itemIds = lines.Select(l => l.InventoryItemId).Distinct().ToList();
                return await inventoryItemRepository.AllExistAsync(itemIds, cancellation);
            })
            .WithMessage("صنف أو أكثر من أصناف المخزون غير موجود")
            .When(x => x.Lines is { Count: > 0 });

        RuleFor(x => x.Lines)
            .MustAsync(async (lines, cancellation) =>
            {
                var itemIds = lines.Select(l => l.InventoryItemId).Distinct().ToList();
                var items = await inventoryItemRepository.FilterAsync(i => itemIds.Contains(i.Id));
                return items.All(i => i.IsActive);
            })
            .WithMessage("لا يمكن إدخال صنف غير نشط إلى المخزن")
            .When(x => x.Lines is { Count: > 0 });

        RuleForEach(x => x.Lines)
            .SetValidator(new PurchaseInvoiceLineCreateDtoValidator());
            
    }
}
