using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartSellingInvoiceService(
    IUnitOfWork _unitOfWork,
    IValidator<SparePartSellingInvoiceCreateDto> _validator,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc,
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
        Guid? itemId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var invoices = (await _unitOfWork.SparePartSellingInvoiceRepository.FilterAsync(
                i => (dateFrom == null || i.InvoiceDate >= dateFrom)
                    && (dateToExclusive == null || i.InvoiceDate < dateToExclusive)
                    && (itemId == null || i.SparePartSellingInvoiceLines.Any(l => l.InventoryItemId == itemId)),
                nameof(SparePartSellingInvoice.SparePartSellingInvoiceLines),
                $"{nameof(SparePartSellingInvoice.SparePartSellingInvoiceLines)}.{nameof(SparePartSellingInvoiceLine.InventoryItem)}"))
            .OrderByDescending(i => i.InvoiceDate)
            .ThenByDescending(i => i.CreatedAt)
            .ToList();

        var items = invoices.Select(invoice => new SparePartSellingInvoiceListItemDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            ClientName = invoice.ClientName,
            TotalAmount = invoice.TotalAmount,
            CreatedAt = invoice.CreatedAt,
            Lines = invoice.SparePartSellingInvoiceLines
                .OrderBy(l => l.InventoryItem?.Name)
                .Select(MapLine)
                .ToList()
        }).ToList();

        return new SparePartSellingInvoiceListDto
        {
            Invoices = items,
            InvoiceCount = items.Count,
            LineCount = items.Sum(i => i.Lines.Count),
            TotalQuantity = items.SelectMany(i => i.Lines).Sum(l => l.Quantity),
            TotalAmount = items.Sum(i => i.TotalAmount)
        };
    }

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
