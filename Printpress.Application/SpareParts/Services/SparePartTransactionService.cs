using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartTransactionService(IUnitOfWork _unitOfWork, IMapper _mapper) : ISparePartTransactionService
{
    public async Task<PagedList<SparePartTransactionDto>> GetByItemIdAsync(Guid itemId, Paging paging, DateTime? dateFrom, DateTime? dateTo, string transactionType)
    {
        var result = await _unitOfWork.SparePartTransactionRepository
            .FilterAsync(paging, x => x.InventoryItemId == itemId
                && (!dateFrom.HasValue || x.CreatedAt >= dateFrom.Value)
                && (!dateTo.HasValue || x.CreatedAt <= dateTo.Value.AddDays(1))
                && (string.IsNullOrEmpty(transactionType) || x.InventoryTransactionType.ToString() == transactionType));

        return new PagedList<SparePartTransactionDto>
        {
            Items = _mapper.Map<List<SparePartTransactionDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }
}
