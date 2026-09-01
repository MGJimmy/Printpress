using AutoMapper;
using FluentValidation;
using Printpress.Domain;
using Printpress.Domain.Entities.Inventory.DomainServices;

namespace Printpress.Application;

internal sealed class PurchaseInvoiceService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<PurchaseInvoiceCreateDto> _createValidator,
    IInventoryTransactionDomainService _inventoryTransactionService,
    IGuidGenerator _guidGenerator,
    CashAccountDomainService _cashAccountDomainService,
    IUserDisplayNameService _userDisplayNameService,
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
        int? categoryId,
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided)
    {
        InvoiceVoidHelper.EnsureDateRange(dateFrom, dateToExclusive, _loc);

        var paging = new Paging(pageNumber, pageSize);
        var sorting = new Sorting(nameof(PurchaseInvoice.InvoiceDate), SortingDirection.DESC);

        var paged = await _unitOfWork.PurchaseInvoiceRepository.FilterAsync(
            paging,
            i => (dateFrom == null || i.InvoiceDate >= dateFrom)
                && (dateToExclusive == null || i.InvoiceDate < dateToExclusive)
                && (itemId == null || i.PurchaseInvoiceLines.Any(l => l.InventoryItemId == itemId))
                && (categoryId == null || i.PurchaseInvoiceLines.Any(l => l.InventoryItem.InventoryItemCategoryId == categoryId))
                && (isVoided == null || i.IsVoided == isVoided),
            sorting);

        var items = paged.Items.Select(MapHeader).ToList();

        return new InventoryPurchaseInvoiceListDto
        {
            Invoices = items,
            InvoiceCount = paged.TotalCount,
            LineCount = 0,
            TotalQuantity = 0,
            TotalAmount = items.Where(i => !i.IsVoided).Sum(i => i.TotalAmount)
        };
    }

    public async Task<InventoryPurchaseInvoiceListItemDto> GetByIdAsync(Guid id)
    {
        var invoice = await _unitOfWork.PurchaseInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(PurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(PurchaseInvoice.PurchaseInvoiceLines)}.{nameof(PurchaseInvoiceLine.InventoryItem)}",
                $"{nameof(PurchaseInvoice.PurchaseInvoiceLines)}.{nameof(PurchaseInvoiceLine.InventoryItem)}.{nameof(InventoryItem.InventoryItemCategory_LKP)}")
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.NotFound));

        var dto = MapHeader(invoice);
        dto.VoidedByName = await InvoiceVoidHelper.ResolveUserNameAsync(_userDisplayNameService, invoice.VoidedBy);
        dto.Lines = invoice.PurchaseInvoiceLines
            .OrderBy(l => l.InventoryItem?.Name)
            .Select(MapLine)
            .ToList();
        return dto;
    }

    public async Task VoidAsync(Guid id, string reason, string userId)
    {
        reason = InvoiceVoidHelper.RequireReason(reason, _loc);

        var invoice = await _unitOfWork.PurchaseInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(PurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(PurchaseInvoice.PurchaseInvoiceLines)}.{nameof(PurchaseInvoiceLine.InventoryItem)}")
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.NotFound));

        if (invoice.IsVoided)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.AlreadyVoided));

        foreach (var group in invoice.PurchaseInvoiceLines.GroupBy(l => l.InventoryItemId))
        {
            var item = await _unitOfWork.InventoryItemRepository.FindByIdWithTransactions(group.Key)
                ?? throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(group.Key));
            var stock = InventoryCalculatorDS.CalculateStockQuantity(item.InventoryTransactions);
            var qty = (int)group.Sum(l => l.Quantity);
            if (stock < qty)
                throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.InsufficientStockToVoid, item.Name));
        }

        var reversals = _inventoryTransactionService.CreatePurchaseVoidTransactions(
            invoice.PurchaseInvoiceLines.ToList(),
            invoice.InvoiceNumber);
        await _unitOfWork.InventoryTransactionRepository.AddRange(reversals);

        await InvoiceVoidHelper.VoidLinkedCashAsync(
            _unitOfWork,
            _cashAccountDomainService,
            _loc,
            CashTransactionReferenceType.PurchaseInventoryInvoice,
            invoice.Id,
            reason);

        invoice.MarkAsVoided(reason, userId);
        _unitOfWork.PurchaseInvoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    private static InventoryPurchaseInvoiceListItemDto MapHeader(PurchaseInvoice invoice) => new()
    {
        Id = invoice.Id,
        InvoiceNumber = invoice.InvoiceNumber,
        InvoiceDate = invoice.InvoiceDate,
        SupplierName = invoice.SupplierName,
        TotalAmount = invoice.TotalAmount,
        AttachmentFilePath = invoice.AttachmentFilePath,
        CreatedAt = invoice.CreatedAt,
        IsVoided = invoice.IsVoided,
        VoidReason = invoice.VoidReason,
        VoidedAt = invoice.VoidedAt,
        VoidedBy = invoice.VoidedBy
    };

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
