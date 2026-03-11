using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartSellingInvoiceService(
    IUnitOfWork _unitOfWork,
    IValidator<SparePartSellingInvoiceCreateDto> _validator,
    IGuidGenerator _guidGenerator) : ISparePartSellingInvoiceService
{
    public async Task<SparePartSellingInvoiceDto> CreateAsync(SparePartSellingInvoiceCreateDto payload, string userId)
    {
        var validationResult = await _validator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        foreach (var line in payload.Lines)
        {
            var item = await _unitOfWork.SparePartItemRepository.FindByIdWithStockQuantityAsync(line.SparePartItemId);
            if (item is null)
                throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(line.SparePartItemId));
            if ((int)line.Quantity > item.StockQuantity)
                throw new ValidationExeption($"الكمية المطلوبة للقطعة '{item.Name}' تتجاوز الكمية المتاحة في المخزون");
        }

        var invoiceId = _guidGenerator.NewGuid();
        var lines = payload.Lines.Select(l => new SparePartSellingInvoiceLine
        {
            Id = _guidGenerator.NewGuid(),
            SellingInvoiceId = invoiceId,
            InventoryItemId = l.SparePartItemId,
            Quantity = l.Quantity,
            UnitPrice = l.UnitPrice,
            LineTotal = l.Quantity * l.UnitPrice
        }).ToList();

        var invoice = new SparePartSellingInvoice
        {
            Id = invoiceId,
            ClientName = payload.ClientName,
            InvoiceDate = payload.InvoiceDate,
            TotalAmount = lines.Sum(l => l.LineTotal),
            SparePartSellingInvoiceLines = lines
        };

        var saved = await _unitOfWork.SparePartSellingInvoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(userId);

        var transactions = lines.Select(l => new SparePartInventoryTransaction(
            l.InventoryItemId,
            SparePartInventoryTransactionType.Out,
            (int)l.Quantity,
            string.Empty) { Id = _guidGenerator.NewGuid() }).ToList();

        await _unitOfWork.SparePartTransactionRepository.AddRange(transactions);
        await _unitOfWork.SaveChangesAsync(userId);

        return new SparePartSellingInvoiceDto
        {
            Id = saved.Id,
            InvoiceNumber = saved.InvoiceNumber,
            ClientName = saved.ClientName,
            InvoiceDate = saved.InvoiceDate,
            TotalAmount = saved.TotalAmount
        };
    }
}
