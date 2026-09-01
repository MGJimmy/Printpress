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

        var paidNow = InvoiceSettlementHelper.ResolvePaidNow(payload.PaidNow, entity.TotalAmount, _loc);
        var receiveNow = payload.ReceiveNow ?? true;
        entity.SetInitialSettlement(paidNow, receiveNow);

        var saved = await _unitOfWork.PurchaseInvoiceRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(userId);

        if (receiveNow)
        {
            var inventoryTransactions = _inventoryTransactionService.CreateInventoryTransaction(entity.PurchaseInvoiceLines.ToList());
            await _unitOfWork.InventoryTransactionRepository.AddRange(inventoryTransactions);
        }

        await InvoiceSettlementHelper.AddCashOutAsync(
            _unitOfWork,
            _cashAccountDomainService,
            _loc,
            CashAccountType.Main,
            CashTransactionReferenceType.PurchaseInventoryInvoice,
            entity.Id,
            paidNow,
            InvoiceSettlementHelper.BuildPaymentDescription(
                _loc,
                LocalizationKeys.CashAccounts.PurchaseInvoiceDescription,
                entity.InvoiceNumber,
                null),
            entity.InvoiceDate);

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
        bool? isVoided,
        bool? hasRemaining,
        bool? isGoodsReceived)
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
                && (isVoided == null || i.IsVoided == isVoided)
                && (hasRemaining == null || (hasRemaining.Value
                    ? !i.IsVoided && i.PaidAmount < i.TotalAmount
                    : i.IsVoided || i.PaidAmount >= i.TotalAmount))
                && (isGoodsReceived == null || i.IsGoodsReceived == isGoodsReceived),
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
        dto.Payments = await InvoiceSettlementHelper.GetPaymentsAsync(
            _unitOfWork,
            CashTransactionReferenceType.PurchaseInventoryInvoice,
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
            CashAccountType.Main,
            CashTransactionReferenceType.PurchaseInventoryInvoice,
            invoice.Id,
            payload.Amount,
            InvoiceSettlementHelper.BuildPaymentDescription(
                _loc,
                LocalizationKeys.CashAccounts.PurchaseInvoiceDescription,
                invoice.InvoiceNumber,
                payload.Note),
            DateTime.UtcNow);

        _unitOfWork.PurchaseInvoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task ReceiveGoodsAsync(Guid id, string userId)
    {
        var invoice = await LoadInvoiceAsync(id);
        invoice.ReceiveGoods();

        var inventoryTransactions = _inventoryTransactionService.CreateInventoryTransaction(invoice.PurchaseInvoiceLines.ToList());
        await _unitOfWork.InventoryTransactionRepository.AddRange(inventoryTransactions);

        _unitOfWork.PurchaseInvoiceRepository.Update(invoice);
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
        }

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

    private async Task<PurchaseInvoice> LoadInvoiceAsync(Guid id)
    {
        return await _unitOfWork.PurchaseInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(PurchaseInvoice.PurchaseInvoiceLines),
                $"{nameof(PurchaseInvoice.PurchaseInvoiceLines)}.{nameof(PurchaseInvoiceLine.InventoryItem)}")
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.NotFound));
    }

    private static InventoryPurchaseInvoiceListItemDto MapHeader(PurchaseInvoice invoice) => new()
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
}
