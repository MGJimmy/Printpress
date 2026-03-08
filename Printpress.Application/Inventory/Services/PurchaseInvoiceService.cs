using AutoMapper;
using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class PurchaseInvoiceService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<PurchaseInvoiceCreateDto> _createValidator,
    IInventoryTransactionDomainService _inventoryTransactionService,
    IGuidGenerator _guidGenerator) : IPurchaseInvoiceService
{
    public async Task<PurchaseInvoiceDto> CreateAsync(PurchaseInvoiceCreateDto payload, string userId)
    {
        var validationResult = await _createValidator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var entity = new PurchaseInvoice(payload.InvoiceNumber, payload.InvoiceDate, payload.SupplierName, payload.AttachmentFilePath);
        entity.Id = _guidGenerator.NewGuid();

        payload.Lines.ForEach(line =>
        {
            entity.AddLine(_guidGenerator.NewGuid(), line.InventoryItemId, line.Quantity, line.UnitPrice);
        });

        var saved = await _unitOfWork.PurchaseInvoiceRepository.AddAsync(entity);

        await _unitOfWork.SaveChangesAsync(userId);

         List<InventoryTransaction> inventoryTransactions = _inventoryTransactionService.CreateInventoryTransaction(entity.PurchaseInvoiceLines.ToList());

        await _unitOfWork.InventoryTransactionRepository.AddRange(inventoryTransactions);

        await _unitOfWork.SaveChangesAsync(userId);

        return _mapper.Map<PurchaseInvoiceDto>(saved);
    }
}
