using AutoMapper;
using FluentValidation;
using Printpress.Domain;
using Printpress.Domain.Entities.Inventory.DomainServices;

namespace Printpress.Application;

internal sealed class InventoryTransactionService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<StockOutCreateDto> _stockOutValidator,
    IGuidGenerator _guidGenerator) : IInventoryTransactionService
{
    public Task<PagedList<InventoryTransactionDto>> GetByItemIdAsync(Guid itemId, Paging paging, DateTime? dateFrom, DateTime? dateTo, string transactionType)
    {
        var result = _unitOfWork.InventoryTransactionRepository
            .Filter(paging,
                x => x.InventoryItemId == itemId
                    && (!dateFrom.HasValue || x.CreatedAt >= dateFrom.Value)
                    && (!dateTo.HasValue || x.CreatedAt <= dateTo.Value.AddDays(1))
                    && (string.IsNullOrEmpty(transactionType) || x.InventoryTransactionType.ToString() == transactionType),
                null,
                nameof(InventoryTransaction.Worker));

        return Task.FromResult(new PagedList<InventoryTransactionDto>
        {
            Items = _mapper.Map<List<InventoryTransactionDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        });
    }

    public Task<PagedList<InventoryTransactionDto>> GetByWorkerIdAsync(
        Guid workerId,
        Paging paging,
        int? inventoryItemCategoryId,
        Guid? inventoryItemId,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        var query = _unitOfWork.InventoryTransactionRepository
            .Filter(
                x => x.WorkerId == workerId,
                nameof(InventoryTransaction.InventoryItem),
                nameof(InventoryTransaction.Worker))
            .AsQueryable();

        if (inventoryItemId.HasValue)
            query = query.Where(x => x.InventoryItemId == inventoryItemId.Value);

        if (inventoryItemCategoryId.HasValue)
            query = query.Where(x => x.InventoryItem != null && x.InventoryItem.InventoryItemCategoryId == inventoryItemCategoryId.Value);

        if (dateFrom.HasValue)
            query = query.Where(x => x.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(x => x.CreatedAt <= dateTo.Value.AddDays(1));

        var totalCount = query.Count();
        var items = query
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(item => _mapper.Map<InventoryTransactionDto>(item))
            .ToList();

        return Task.FromResult(new PagedList<InventoryTransactionDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = paging.PageNumber,
            PageSize = paging.PageSize
        });
    }

    public async Task StockOutAsync(StockOutCreateDto payload, string userId)
    {
        var validationResult = await _stockOutValidator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var item = await _unitOfWork.InventoryItemRepository.FindByIdWithTransactions(payload.InventoryItemId);
        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.InventoryItemId));

        if (!item.IsActive)
            throw new ValidationExeption("لا يمكن صرف صنف غير نشط من المخزن");

        var stockQuantity = InventoryCalculatorDS.CalculateStockQuantity(item.InventoryTransactions);


        if (payload.Quantity > stockQuantity)
            throw new ValidationExeption("الكمية المطلوبة تتجاوز الكمية المتاحة في المخزون");

        var transaction = new InventoryTransaction(
            payload.InventoryItemId,
            InventoryTransactionType.Out,
            payload.Quantity,
            InventoryTransactionReferenceType.StockAdjustment,
            _guidGenerator.NewGuid(),
            payload.Notes ?? string.Empty);
        transaction.Id = _guidGenerator.NewGuid();
        transaction.WorkerId = payload.WorkerId;

        await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task<InventoryTransactionListDto> GetAllAsync(
        int? categoryId,
        Guid? itemId,
        Guid? workerId,
        InventoryTransactionType? type,
        DateTime? dateFrom,
        DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var transactions = (await _unitOfWork.InventoryTransactionRepository.FilterAsync(
                t => (dateFrom == null || t.CreatedAt >= dateFrom)
                    && (dateToExclusive == null || t.CreatedAt < dateToExclusive)
                    && (itemId == null || t.InventoryItemId == itemId)
                    && (categoryId == null || t.InventoryItem.InventoryItemCategoryId == categoryId)
                    && (workerId == null || t.WorkerId == workerId)
                    && (type == null || t.InventoryTransactionType == type),
                nameof(InventoryTransaction.InventoryItem),
                $"{nameof(InventoryTransaction.InventoryItem)}.{nameof(InventoryItem.InventoryItemCategory_LKP)}",
                nameof(InventoryTransaction.Worker)))
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        var purchaseLineIds = transactions
            .Where(t => t.ReferenceType == InventoryTransactionReferenceType.Purchase)
            .Select(t => t.ReferenceId)
            .Distinct()
            .ToList();

        var purchaseLines = purchaseLineIds.Count == 0
            ? []
            : (await _unitOfWork.PurchaseInvoiceLineRepository.FilterAsync(
                l => purchaseLineIds.Contains(l.Id),
                nameof(PurchaseInvoiceLine.PurchaseInvoice)))
                .ToDictionary(l => l.Id);

        var rows = transactions.Select(t =>
        {
            var (label, route) = ResolveReference(t, purchaseLines);
            return new InventoryTransactionListRowDto
            {
                Id = t.Id,
                CreatedAt = t.CreatedAt,
                ItemId = t.InventoryItemId,
                ItemName = t.InventoryItem?.Name ?? "—",
                CategoryName = t.InventoryItem?.InventoryItemCategory_LKP?.Name ?? "—",
                InventoryTransactionType = t.InventoryTransactionType,
                Quantity = t.Quantity,
                ReferenceType = t.ReferenceType,
                ReferenceLabel = label,
                ReferenceRoute = route,
                WorkerId = t.WorkerId,
                WorkerName = t.Worker?.Name,
                Notes = t.Notes
            };
        }).ToList();

        return new InventoryTransactionListDto
        {
            Rows = rows,
            MovementCount = rows.Count,
            ItemCount = rows.Select(r => r.ItemId).Distinct().Count(),
            TotalInQuantity = rows.Where(r => r.InventoryTransactionType == InventoryTransactionType.In).Sum(r => r.Quantity),
            TotalOutQuantity = rows.Where(r => r.InventoryTransactionType == InventoryTransactionType.Out).Sum(r => r.Quantity)
        };
    }

    private static (string Label, string Route) ResolveReference(
        InventoryTransaction transaction,
        Dictionary<Guid, PurchaseInvoiceLine> purchaseLines)
    {
        return transaction.ReferenceType switch
        {
            InventoryTransactionReferenceType.Purchase when purchaseLines.TryGetValue(transaction.ReferenceId, out var line)
                => (
                    $"فاتورة شراء: {line.PurchaseInvoice?.InvoiceNumber ?? "—"}",
                    $"/inventory/stock-in/invoices/{line.PurchaseInvoiceId}"),
            InventoryTransactionReferenceType.Purchase
                => ("فاتورة شراء", ""),
            InventoryTransactionReferenceType.Order
                => ("طلب", $"/order/view/{transaction.ReferenceId}"),
            InventoryTransactionReferenceType.StockAdjustment
                => ("صرف يدوي", ""),
            _ => ("—", "")
        };
    }
}
