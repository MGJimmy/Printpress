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
        Paging paging = new Paging
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        Sorting sorting = new Sorting
        {
            Field = "Id",
            Dir = SortingDirection.DESC 
        };

        PagedList<OrderTransaction> pagedList = _unitOfWork.OrderTransactionRepository.All(paging, sorting);

        // check if no data returned then return no data founds

        var result = _orderTransactionMapper.MapFromSourceToDestination(pagedList);

        return await Task.FromResult(result);
    }
}
