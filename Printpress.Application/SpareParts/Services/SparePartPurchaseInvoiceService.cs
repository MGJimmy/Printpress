using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartPurchaseInvoiceService(
    IUnitOfWork _unitOfWork,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc,
    IUserDisplayNameService _userDisplayNameService,
    CashAccountDomainService _cashAccountDomainService) : ISparePartPurchaseInvoiceService
{
    public async Task CreateAsync(SparePartPurchaseInvoiceCreateDto payload, string userId)
    {
        var invoice = new SparePartPurchaseInvoice(
            payload.InvoiceNumber,
            UtcDateTime.AsUtc(payload.InvoiceDate),
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
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided)
    {
        InvoiceVoidHelper.EnsureDateRange(dateFrom, dateToExclusive, _loc);

        var paging = new Paging(pageNumber, pageSize);
        var sorting = new Sorting(nameof(SparePartPurchaseInvoice.InvoiceDate), SortingDirection.DESC);

        var paged = await _unitOfWork.SparePartPurchaseInvoiceRepository.FilterAsync(
            paging,
            i => (dateFrom == null || i.InvoiceDate >= dateFrom)
                && (dateToExclusive == null || i.InvoiceDate < dateToExclusive)
                && (itemId == null || i.PurchaseInvoiceLines.Any(l => l.InventoryItemId == itemId))
                && (isVoided == null || i.IsVoided == isVoided),
            sorting);

        var items = paged.Items.Select(MapHeader).ToList();

        return new SparePartPurchaseInvoiceListDto
        {
            Invoices = items,
            InvoiceCount = paged.TotalCount,
            LineCount = 0,
            TotalQuantity = 0,
            TotalAmount = items.Where(i => !i.IsVoided).Sum(i => i.TotalAmount)
        };
    }

    public async Task<SparePartPurchaseInvoiceListItemDto> GetByIdAsync(Guid id)
    {
        var invoice = await _unitOfWork.SparePartPurchaseInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines)}.{nameof(SparePartPurchaseInvoiceLine.InventoryItem)}")
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

        var invoice = await _unitOfWork.SparePartPurchaseInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines)}.{nameof(SparePartPurchaseInvoiceLine.InventoryItem)}")
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.NotFound));

        if (invoice.IsVoided)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.AlreadyVoided));

        foreach (var group in invoice.PurchaseInvoiceLines.GroupBy(l => l.InventoryItemId))
        {
            var item = await _unitOfWork.SparePartItemRepository.FindByIdWithStockQuantityAsync(group.Key)
                ?? throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(group.Key));
            var qty = (int)group.Sum(l => l.Quantity);
            if (item.StockQuantity < qty)
                throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.InsufficientStockToVoid, item.Name));
        }

        var reversals = invoice.PurchaseInvoiceLines.Select(line => new SparePartInventoryTransaction(
            line.InventoryItemId,
            SparePartInventoryTransactionType.Out,
            (int)line.Quantity,
            $"إلغاء فاتورة شراء {invoice.InvoiceNumber}")
        { Id = _guidGenerator.NewGuid() }).ToList();

        await _unitOfWork.SparePartTransactionRepository.AddRange(reversals);

        await InvoiceVoidHelper.VoidLinkedCashAsync(
            _unitOfWork,
            _cashAccountDomainService,
            _loc,
            CashTransactionReferenceType.PurchaseSparePartInvoice,
            invoice.Id,
            reason);

        invoice.MarkAsVoided(reason, userId);
        _unitOfWork.SparePartPurchaseInvoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    private static SparePartPurchaseInvoiceListItemDto MapHeader(SparePartPurchaseInvoice invoice) => new()
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
