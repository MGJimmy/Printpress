using AutoMapper;
using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class PurchaseInvoiceService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<PurchaseInvoiceCreateDto> _createValidator,
    IInventoryTransactionDomainService _inventoryTransactionService,
    IGuidGenerator _guidGenerator,
    CashAccountDomainService _cashAccountDomainService,
    ILocalizationService _loc) : IPurchaseInvoiceService
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

        await AddCashAccountTransaction(entity);

        await _unitOfWork.SaveChangesAsync(userId);

        return _mapper.Map<PurchaseInvoiceDto>(saved);
    }

    private async Task AddCashAccountTransaction(PurchaseInvoice purchaseInvoice)
    {
        var cashAccount = await _unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == CashAccountType.Main)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        _cashAccountDomainService.AddCashAccountTransaction(
            cashAccount,
            CashTransactionType.Out,
            CashTransactionCategory.Purchases,
            CashTransactionReferenceType.PurchaseInventoryInvoice,
            purchaseInvoice.Id,
            purchaseInvoice.TotalAmount,
            _loc.Get(LocalizationKeys.CashAccounts.PurchaseInvoiceDescription, purchaseInvoice.InvoiceNumber),
            purchaseInvoice.InvoiceDate
        );

        _unitOfWork.CashAccountRepository.Update(cashAccount);
    }
}
