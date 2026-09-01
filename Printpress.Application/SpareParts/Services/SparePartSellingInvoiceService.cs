using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartSellingInvoiceService(
    IUnitOfWork _unitOfWork,
    IValidator<SparePartSellingInvoiceCreateDto> _validator,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc,
    IUserDisplayNameService _userDisplayNameService,
    CashAccountDomainService _cashAccountDomainService) : ISparePartSellingInvoiceService
{
    public async Task<SparePartSellingInvoiceDto> CreateAsync(SparePartSellingInvoiceCreateDto payload, string userId)
    {
        var validationResult = await _validator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        await ValidateStockQuantity(payload.Lines);


        var invoice = new SparePartSellingInvoice(
                default,
                payload.ClientName,
                UtcDateTime.AsUtc(payload.InvoiceDate));

        foreach (var line in payload.Lines)
        {
            invoice.AddLine(line.SparePartItemId, line.Quantity, line.UnitPrice);
        }

        await _unitOfWork.SparePartSellingInvoiceRepository.AddAsync(invoice);

        var transactions = invoice.SparePartSellingInvoiceLines.Select(l => new SparePartInventoryTransaction(
            l.InventoryItemId,
            SparePartInventoryTransactionType.Out,
            (int)l.Quantity,
            string.Empty) { Id = _guidGenerator.NewGuid() }).ToList();

        await _unitOfWork.SparePartTransactionRepository.AddRange(transactions);


        await AddCashAccountTransaction(invoice);

        await _unitOfWork.SaveChangesAsync(userId);

        return new SparePartSellingInvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            ClientName = invoice.ClientName,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount
        };
    }

    public async Task<SparePartSellingInvoiceListDto> GetAllAsync(
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided)
    {
        InvoiceVoidHelper.EnsureDateRange(dateFrom, dateToExclusive, _loc);

        var paging = new Paging(pageNumber, pageSize);
        var sorting = new Sorting(nameof(SparePartSellingInvoice.InvoiceDate), SortingDirection.DESC);

        var paged = await _unitOfWork.SparePartSellingInvoiceRepository.FilterAsync(
            paging,
            i => (dateFrom == null || i.InvoiceDate >= dateFrom)
                && (dateToExclusive == null || i.InvoiceDate < dateToExclusive)
                && (itemId == null || i.SparePartSellingInvoiceLines.Any(l => l.InventoryItemId == itemId))
                && (isVoided == null || i.IsVoided == isVoided),
            sorting);

        var items = paged.Items.Select(MapHeader).ToList();

        return new SparePartSellingInvoiceListDto
        {
            Invoices = items,
            InvoiceCount = paged.TotalCount,
            LineCount = 0,
            TotalQuantity = 0,
            TotalAmount = items.Where(i => !i.IsVoided).Sum(i => i.TotalAmount)
        };
    }

    public async Task<SparePartSellingInvoiceListItemDto> GetByIdAsync(Guid id)
    {
        var invoice = await _unitOfWork.SparePartSellingInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(SparePartSellingInvoice.SparePartSellingInvoiceLines),
                $"{nameof(SparePartSellingInvoice.SparePartSellingInvoiceLines)}.{nameof(SparePartSellingInvoiceLine.InventoryItem)}")
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.NotFound));

        var dto = MapHeader(invoice);
        dto.VoidedByName = await InvoiceVoidHelper.ResolveUserNameAsync(_userDisplayNameService, invoice.VoidedBy);
        dto.Lines = invoice.SparePartSellingInvoiceLines
            .OrderBy(l => l.InventoryItem?.Name)
            .Select(MapLine)
            .ToList();
        return dto;
    }

    public async Task VoidAsync(Guid id, string reason, string userId)
    {
        reason = InvoiceVoidHelper.RequireReason(reason, _loc);

        var invoice = await _unitOfWork.SparePartSellingInvoiceRepository.FirstOrDefaultAsync(
                i => i.Id == id,
                true,
                nameof(SparePartSellingInvoice.SparePartSellingInvoiceLines))
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.NotFound));

        if (invoice.IsVoided)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Invoices.AlreadyVoided));

        var restorals = invoice.SparePartSellingInvoiceLines.Select(line => new SparePartInventoryTransaction(
            line.InventoryItemId,
            SparePartInventoryTransactionType.In,
            (int)line.Quantity,
            $"إلغاء فاتورة بيع {invoice.InvoiceNumber}")
        { Id = _guidGenerator.NewGuid() }).ToList();

        await _unitOfWork.SparePartTransactionRepository.AddRange(restorals);

        await InvoiceVoidHelper.VoidLinkedCashAsync(
            _unitOfWork,
            _cashAccountDomainService,
            _loc,
            CashTransactionReferenceType.SellingSparePartInvoice,
            invoice.Id,
            reason);

        invoice.MarkAsVoided(reason, userId);
        _unitOfWork.SparePartSellingInvoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    private static SparePartSellingInvoiceListItemDto MapHeader(SparePartSellingInvoice invoice) => new()
    {
        Id = invoice.Id,
        InvoiceNumber = invoice.InvoiceNumber,
        InvoiceDate = invoice.InvoiceDate,
        ClientName = invoice.ClientName,
        TotalAmount = invoice.TotalAmount,
        CreatedAt = invoice.CreatedAt,
        IsVoided = invoice.IsVoided,
        VoidReason = invoice.VoidReason,
        VoidedAt = invoice.VoidedAt,
        VoidedBy = invoice.VoidedBy
    };

    private static SparePartInvoiceLineDto MapLine(SparePartSellingInvoiceLine line) => new()
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

    private async Task ValidateStockQuantity(List<SparePartSellingInvoiceLineCreateDto> lines)
    {
        foreach (var line in lines)
        {
            var item = await _unitOfWork.SparePartItemRepository.FindByIdWithStockQuantityAsync(line.SparePartItemId);
            if (item is null)
                throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(line.SparePartItemId));
            if ((int)line.Quantity > item.StockQuantity)
                throw new ValidationExeption($"الكمية المطلوبة للقطعة '{item.Name}' تتجاوز الكمية المتاحة في المخزون");
        }
    }

    private async Task AddCashAccountTransaction(SparePartSellingInvoice invoice)
    {
        var cashAccount = await _unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == CashAccountType.SpareParts)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        _cashAccountDomainService.AddCashAccountTransaction(
            cashAccount,
            CashTransactionType.In,
            CashTransactionCategory.Sales,
            CashTransactionReferenceType.SellingSparePartInvoice,
            invoice.Id,
            invoice.TotalAmount,
            _loc.Get(LocalizationKeys.CashAccounts.SpareSellingInvoiceDescription, invoice.InvoiceNumber),
            invoice.InvoiceDate
        );

        _unitOfWork.CashAccountRepository.Update(cashAccount);
    }
}
