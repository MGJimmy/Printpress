using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class InventoryTransactionService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IInventoryTransactionService
{
    public async Task<PagedList<InventoryTransactionDto>> GetByItemIdAsync(Guid itemId, Paging paging)
    {
        var result = await _unitOfWork.InventoryTransactionRepository
            .FilterAsync(paging, x => x.InventoryItemId == itemId);

        return new PagedList<InventoryTransactionDto>
        {
            Items = _mapper.Map<List<InventoryTransactionDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }
}
