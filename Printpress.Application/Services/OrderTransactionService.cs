using Printpress.Domain.Entities;

namespace Printpress.Application;

public class OrderTransactionService(IUnitOfWork _unitOfWork, OrderTransactionMapper _orderTransactionMapper) : IOrderTransactionService
{
    public async Task<OrderTransactionDto> AddAsync(OrderTransactionAddDto payload)
    {
        // Make validation
        // validate tranactiontype string is valid enum value by enum helper

        var client = await _unitOfWork.OrderTransactionRepository.AddAsync(_orderTransactionMapper.MapFromDestinationToSource(payload));


        await _unitOfWork.SaveChangesAsync();

        return _orderTransactionMapper.MapFromSourceToDestination(client);
    }

    public async Task<PagedList<OrderTransactionDto>> GetByPage(int pageNumber, int pageSize)
    {

        PagedList<OrderTransaction> pagedList = await _unitOfWork.OrderTransactionRepository.AllAsync(
            new Paging(pageNumber, pageSize),
            new Sorting(nameof(OrderTransaction.Id), SortingDirection.DESC)
            );

        // check if no data returned then return no data founds

        var result = _orderTransactionMapper.MapFromSourceToDestination(pagedList);

        return result;
    }
}
