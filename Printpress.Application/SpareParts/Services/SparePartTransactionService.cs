using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartTransactionService(IUnitOfWork _unitOfWork, IMapper _mapper) : ISparePartTransactionService
{
    public async Task<PagedList<SparePartTransactionDto>> GetByItemIdAsync(
        Guid itemId,
        Paging paging,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        string transactionType)
    {
        var sorting = new Sorting(nameof(SparePartInventoryTransaction.CreatedAt), SortingDirection.DESC);

        var result = await _unitOfWork.SparePartTransactionRepository.FilterAsync(
            paging,
            x => x.InventoryItemId == itemId
                && (!dateFrom.HasValue || x.CreatedAt >= dateFrom.Value)
                && (!dateToExclusive.HasValue || x.CreatedAt < dateToExclusive.Value)
                && (string.IsNullOrEmpty(transactionType) || x.InventoryTransactionType.ToString() == transactionType),
            sorting);

        return new PagedList<SparePartTransactionDto>
        {
            Items = _mapper.Map<List<SparePartTransactionDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }
}
