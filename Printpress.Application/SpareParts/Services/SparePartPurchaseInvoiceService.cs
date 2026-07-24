using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartPurchaseInvoiceService(
    IUnitOfWork _unitOfWork,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc,
    CachAccountDomainService _cachAccountDomainService) : ISparePartPurchaseInvoiceService
{
    public async Task CreateAsync(SparePartPurchaseInvoiceCreateDto payload, string userId)
    {
        var invoice = new SparePartPurchaseInvoice(
            payload.InvoiceNumber,
            payload.InvoiceDate,
            payload.SupplierName,
            payload.AttachmentFilePath ?? string.Empty);

        foreach (var line in payload.Lines)
        {
            invoice.AddLine(_guidGenerator.NewGuid(), line.SparePartItemId, line.Quantity, line.UnitPrice);
        }

        await _unitOfWork.SparePartPurchaseInvoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(userId);

        var transactions = payload.Lines.Select(line => new SparePartInventoryTransaction(
            line.SparePartItemId,
            SparePartInventoryTransactionType.In,
            (int)line.Quantity,
            string.Empty) { Id = _guidGenerator.NewGuid() }).ToList();

        await _unitOfWork.SparePartTransactionRepository.AddRange(transactions);

        await AddCachAccountTransaction(invoice);

        await _unitOfWork.SaveChangesAsync(userId);
    }

    private async Task AddCachAccountTransaction(SparePartPurchaseInvoice invoice)
    {

        var cachAccount = await _unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == CashAccountType.SpareParts)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        _cachAccountDomainService.AddCachAccountTransaction(
            cachAccount,
            CashTransactionType.Out,
            CashTransactionCategory.Purchases,
            CashTransactionReferenceType.PurchaseSparePartInvoice,
            invoice.Id,
            invoice.TotalAmount,
            $"Purchase Invoice Line: {invoice.InvoiceNumber}"
        );
    }
}
