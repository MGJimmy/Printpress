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
    public async Task<PagedList<InventoryTransactionDto>> GetByItemIdAsync(Guid itemId, Paging paging, DateTime? dateFrom, DateTime? dateTo, string transactionType)
    {
        var result = await _unitOfWork.InventoryTransactionRepository
            .FilterAsync(paging, x => x.InventoryItemId == itemId
                && (!dateFrom.HasValue || x.CreatedAt >= dateFrom.Value)
                && (!dateTo.HasValue || x.CreatedAt <= dateTo.Value.AddDays(1))
                && (string.IsNullOrEmpty(transactionType) || x.InventoryTransactionType.ToString() == transactionType));

        return new PagedList<InventoryTransactionDto>
        {
            Items = _mapper.Map<List<InventoryTransactionDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
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

        await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync(userId);
    }
}
