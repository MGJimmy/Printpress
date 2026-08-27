using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartPurchaseInvoiceService(
    IUnitOfWork _unitOfWork,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc,
    CashAccountDomainService _cashAccountDomainService) : ISparePartPurchaseInvoiceService
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

        await AddCashAccountTransaction(invoice);

        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task<SparePartPurchaseInvoiceListDto> GetAllAsync(
        Guid? itemId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var invoices = (await _unitOfWork.SparePartPurchaseInvoiceRepository.FilterAsync(
                i => (dateFrom == null || i.InvoiceDate >= dateFrom)
                    && (dateToExclusive == null || i.InvoiceDate < dateToExclusive)
                    && (itemId == null || i.PurchaseInvoiceLines.Any(l => l.InventoryItemId == itemId)),
                nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines)}.{nameof(SparePartPurchaseInvoiceLine.InventoryItem)}"))
            .OrderByDescending(i => i.InvoiceDate)
            .ThenByDescending(i => i.CreatedAt)
            .ToList();

        var items = invoices.Select(invoice => new SparePartPurchaseInvoiceListItemDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            SupplierName = invoice.SupplierName,
            TotalAmount = invoice.TotalAmount,
            AttachmentFilePath = invoice.AttachmentFilePath,
            CreatedAt = invoice.CreatedAt,
            Lines = invoice.PurchaseInvoiceLines
                .OrderBy(l => l.InventoryItem?.Name)
                .Select(MapLine)
                .ToList()
        }).ToList();

        return new SparePartPurchaseInvoiceListDto
        {
            Invoices = items,
            InvoiceCount = items.Count,
            LineCount = items.Sum(i => i.Lines.Count),
            TotalQuantity = items.SelectMany(i => i.Lines).Sum(l => l.Quantity),
            TotalAmount = items.Sum(i => i.TotalAmount)
        };
    }

    private static SparePartInvoiceLineDto MapLine(SparePartPurchaseInvoiceLine line) => new()
    {
        Id = line.Id,
        ItemId = line.InventoryItemId,
        ItemName = line.InventoryItem?.Name ?? "—",
        PacksPerCarton = line.InventoryItem?.PacksPerCarton,
        UnitsPerPack = line.InventoryItem?.UnitsPerPack,
        Quantity = line.Quantity,
        UnitPrice = line.UnitPrice,
        LineTotal = line.LineTotal
    };

    private async Task AddCashAccountTransaction(SparePartPurchaseInvoice invoice)
    {
        var cashAccount = await _unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == CashAccountType.SpareParts)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        _cashAccountDomainService.AddCashAccountTransaction(
            cashAccount,
            CashTransactionType.Out,
            CashTransactionCategory.Purchases,
            CashTransactionReferenceType.PurchaseSparePartInvoice,
            invoice.Id,
            invoice.TotalAmount,
            _loc.Get(LocalizationKeys.CashAccounts.SparePurchaseInvoiceDescription, invoice.InvoiceNumber),
            invoice.InvoiceDate
        );

        _unitOfWork.CashAccountRepository.Update(cashAccount);
    }
}
