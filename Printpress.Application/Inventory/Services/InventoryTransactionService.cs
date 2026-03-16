using AutoMapper;
using FluentValidation;
using Printpress.Domain;

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

        var item = await _unitOfWork.InventoryItemRepository.FindByIdWithStockQuantity(payload.InventoryItemId);
        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.InventoryItemId));

        if (payload.Quantity > item.StockQuantity)
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
}
