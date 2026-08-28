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

        var entity = new PurchaseInvoice(payload.InvoiceNumber, UtcDateTime.AsUtc(payload.InvoiceDate), payload.SupplierName, payload.AttachmentFilePath);
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

    public async Task<InventoryPurchaseInvoiceListDto> GetAllAsync(
        int? categoryId, Guid? itemId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var invoices = (await _unitOfWork.PurchaseInvoiceRepository.FilterAsync(
                i => (dateFrom == null || i.InvoiceDate >= dateFrom)
                    && (dateToExclusive == null || i.InvoiceDate < dateToExclusive)
                    && (itemId == null || i.PurchaseInvoiceLines.Any(l => l.InventoryItemId == itemId))
                    && (categoryId == null || i.PurchaseInvoiceLines.Any(l => l.InventoryItem.InventoryItemCategoryId == categoryId)),
                nameof(PurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(PurchaseInvoice.PurchaseInvoiceLines)}.{nameof(PurchaseInvoiceLine.InventoryItem)}",
                $"{nameof(PurchaseInvoice.PurchaseInvoiceLines)}.{nameof(PurchaseInvoiceLine.InventoryItem)}.{nameof(InventoryItem.InventoryItemCategory_LKP)}"))
            .OrderByDescending(i => i.InvoiceDate)
            .ThenByDescending(i => i.CreatedAt)
            .ToList();

        var items = invoices.Select(invoice => new InventoryPurchaseInvoiceListItemDto
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

        return new InventoryPurchaseInvoiceListDto
        {
            Invoices = items,
            InvoiceCount = items.Count,
            LineCount = items.Sum(i => i.Lines.Count),
            TotalQuantity = items.SelectMany(i => i.Lines).Sum(l => l.Quantity),
            TotalAmount = items.Sum(i => i.TotalAmount)
        };
    }

    private static InventoryPurchaseInvoiceLineDto MapLine(PurchaseInvoiceLine line) => new()
    {
        Id = line.Id,
        ItemId = line.InventoryItemId,
        ItemName = line.InventoryItem?.Name ?? "—",
        CategoryName = line.InventoryItem?.InventoryItemCategory_LKP?.Name ?? "—",
        PacksPerCarton = line.InventoryItem?.PacksPerCarton,
        UnitsPerPack = line.InventoryItem?.UnitsPerPack,
        Quantity = line.Quantity,
        UnitPrice = line.UnitPrice,
        LineTotal = line.LineTotal
    };

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
