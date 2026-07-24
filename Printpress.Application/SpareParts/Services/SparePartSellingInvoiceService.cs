using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartSellingInvoiceService(
    IUnitOfWork _unitOfWork,
    IValidator<SparePartSellingInvoiceCreateDto> _validator,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc,
    CachAccountDomainService _cachAccountDomainService) : ISparePartSellingInvoiceService
{
    public async Task<SparePartSellingInvoiceDto> CreateAsync(SparePartSellingInvoiceCreateDto payload, string userId)
    {
        var validationResult = await _validator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        await ValidateStockQuantity(payload.Lines);


        var invoice = new SparePartSellingInvoice(
                default,
                payload.ClientName,
                payload.InvoiceDate);

        foreach (var line in payload.Lines)
        {
            invoice.AddLine(line.SparePartItemId, line.Quantity, line.UnitPrice);
        }

        await _unitOfWork.SparePartSellingInvoiceRepository.AddAsync(invoice);

        var transactions = invoice.SparePartSellingInvoiceLines.Select(l => new SparePartInventoryTransaction(
            l.InventoryItemId,
            SparePartInventoryTransactionType.Out,
            (int)l.Quantity,
            string.Empty) { Id = _guidGenerator.NewGuid() }).ToList();

        await _unitOfWork.SparePartTransactionRepository.AddRange(transactions);


        await AddCachAccountTransaction(invoice);

        await _unitOfWork.SaveChangesAsync(userId);

        return new SparePartSellingInvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            ClientName = invoice.ClientName,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount
        };
    }

    private async Task ValidateStockQuantity(List<SparePartSellingInvoiceLineCreateDto> lines)
    {
        foreach (var line in lines)
        {
            var item = await _unitOfWork.SparePartItemRepository.FindByIdWithStockQuantityAsync(line.SparePartItemId);
            if (item is null)
                throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(line.SparePartItemId));
            if ((int)line.Quantity > item.StockQuantity)
                throw new ValidationExeption($"الكمية المطلوبة للقطعة '{item.Name}' تتجاوز الكمية المتاحة في المخزون");
        }
    }

    private async Task AddCachAccountTransaction(SparePartSellingInvoice invoice)
    {

        var cachAccount = await _unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == CashAccountType.SpareParts)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        _cachAccountDomainService.AddCachAccountTransaction(
            cachAccount,
            CashTransactionType.In,
            CashTransactionCategory.Sales,
            CashTransactionReferenceType.SellingSparePartInvoice,
            invoice.Id,
            invoice.TotalAmount,
            $"Purchase Invoice Line: {invoice.InvoiceNumber}"
        );
    }
}
