using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartPurchaseInvoiceService(
    IUnitOfWork _unitOfWork,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc,
    IUserDisplayNameService _userDisplayNameService,
    CashAccountDomainService _cashAccountDomainService) : ISparePartPurchaseInvoiceService
{
    public async Task<Guid> CreateAsync(SparePartPurchaseInvoiceCreateDto payload, string userId)
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

        var paidNow = InvoiceSettlementHelper.ResolvePaidNow(payload.PaidNow, invoice.TotalAmount, _loc);
        var receiveNow = payload.ReceiveNow ?? true;
        invoice.SetInitialSettlement(paidNow, receiveNow);

        await _unitOfWork.SparePartPurchaseInvoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(userId);

        if (receiveNow)
            await AddStockInAsync(invoice);

        await InvoiceSettlementHelper.AddCashOutAsync(
            _unitOfWork,
            _cashAccountDomainService,
            _loc,
            CashAccountType.SpareParts,
            CashTransactionReferenceType.PurchaseSparePartInvoice,
            invoice.Id,
            paidNow,
            InvoiceSettlementHelper.BuildPaymentDescription(
                _loc,
                LocalizationKeys.CashAccounts.SparePurchaseInvoiceDescription,
                invoice.InvoiceNumber,
                null),
            invoice.InvoiceDate);

        await _unitOfWork.SaveChangesAsync(userId);
        return invoice.Id;
    }

    public async Task<SparePartPurchaseInvoiceListDto> GetAllAsync(
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided,
        bool? hasRemaining,
        bool? isGoodsReceived)
    {
        InvoiceVoidHelper.EnsureDateRange(dateFrom, dateToExclusive, _loc);

        var paging = new Paging(pageNumber, pageSize);
        var sorting = new Sorting(nameof(SparePartPurchaseInvoice.InvoiceDate), SortingDirection.DESC);

        var paged = await _unitOfWork.SparePartPurchaseInvoiceRepository.FilterAsync(
            paging,
            i => (dateFrom == null || i.InvoiceDate >= dateFrom)
                && (dateToExclusive == null || i.InvoiceDate < dateToExclusive)
                && (itemId == null || i.PurchaseInvoiceLines.Any(l => l.InventoryItemId == itemId))
                && (isVoided == null || i.IsVoided == isVoided)
                && (hasRemaining == null || (hasRemaining.Value
                    ? !i.IsVoided && i.PaidAmount < i.TotalAmount
                    : i.IsVoided || i.PaidAmount >= i.TotalAmount))
                && (isGoodsReceived == null || i.IsGoodsReceived == isGoodsReceived),
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
        var invoice = await LoadInvoiceAsync(id);
        var dto = MapHeader(invoice);
        dto.VoidedByName = await InvoiceVoidHelper.ResolveUserNameAsync(_userDisplayNameService, invoice.VoidedBy);
        dto.Payments = await InvoiceSettlementHelper.GetPaymentsAsync(
            _unitOfWork,
            CashTransactionReferenceType.PurchaseSparePartInvoice,
            invoice.Id);
        dto.Lines = invoice.PurchaseInvoiceLines
            .OrderBy(l => l.InventoryItem?.Name)
            .Select(MapLine)
            .ToList();
        return dto;
    }

    public async Task PayAsync(Guid id, InvoicePayDto payload, string userId)
    {
        var invoice = await LoadInvoiceAsync(id);
        if (invoice.IsVoided)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.AlreadyVoided));
        if (invoice.PaidAmount >= invoice.TotalAmount)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.AlreadyFullyPaid));
        if (payload is null || payload.Amount <= 0)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.PaymentAmountInvalid));

        invoice.ApplyPayment(payload.Amount);
        await InvoiceSettlementHelper.AddCashOutAsync(
            _unitOfWork,
            _cashAccountDomainService,
            _loc,
            CashAccountType.SpareParts,
            CashTransactionReferenceType.PurchaseSparePartInvoice,
            invoice.Id,
            payload.Amount,
            InvoiceSettlementHelper.BuildPaymentDescription(
                _loc,
                LocalizationKeys.CashAccounts.SparePurchaseInvoiceDescription,
                invoice.InvoiceNumber,
                payload.Note),
            DateTime.UtcNow);

        _unitOfWork.SparePartPurchaseInvoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task ReceiveGoodsAsync(Guid id, string userId)
    {
        var invoice = await LoadInvoiceAsync(id);
        invoice.ReceiveGoods();
        await AddStockInAsync(invoice);
        _unitOfWork.SparePartPurchaseInvoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task VoidAsync(Guid id, string reason, string userId)
    {
        reason = InvoiceVoidHelper.RequireReason(reason, _loc);

        var invoice = await LoadInvoiceAsync(id);

        if (invoice.IsVoided)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.AlreadyVoided));

        if (invoice.IsGoodsReceived)
        {
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
        }

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

    private async Task AddStockInAsync(SparePartPurchaseInvoice invoice)
    {
        var transactions = invoice.PurchaseInvoiceLines.Select(line => new SparePartInventoryTransaction(
            line.InventoryItemId,
            SparePartInventoryTransactionType.In,
            (int)line.Quantity,
            string.Empty) { Id = _guidGenerator.NewGuid() }).ToList();

        await _unitOfWork.SparePartTransactionRepository.AddRange(transactions);
    }

    private async Task<SparePartPurchaseInvoice> LoadInvoiceAsync(Guid id)
    {
        return await _unitOfWork.SparePartPurchaseInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(SparePartPurchaseInvoice.PurchaseInvoiceLines)}.{nameof(SparePartPurchaseInvoiceLine.InventoryItem)}")
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.NotFound));
    }

    private static SparePartPurchaseInvoiceListItemDto MapHeader(SparePartPurchaseInvoice invoice) => new()
    {
        Id = invoice.Id,
        InvoiceNumber = invoice.InvoiceNumber,
        InvoiceDate = invoice.InvoiceDate,
        SupplierName = invoice.SupplierName,
        TotalAmount = invoice.TotalAmount,
        PaidAmount = invoice.PaidAmount,
        RemainingAmount = invoice.IsVoided ? 0 : invoice.TotalAmount - invoice.PaidAmount,
        IsGoodsReceived = invoice.IsGoodsReceived,
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
}
