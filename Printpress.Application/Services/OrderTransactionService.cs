using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application;

public class OrderTransactionService(IUnitOfWork _unitOfWork, OrderTransactionMapper _orderTransactionMapper) : IOrderTransactionService
{
    public async Task<OrderTransactionDto> AddAsync(OrderTransactionAddDto payload)
    {
        ValidateTransactionPayload(payload);

        var client = await _unitOfWork.OrderTransactionRepository.AddAsync(_orderTransactionMapper.MapFromDestinationToSource(payload));


        await _unitOfWork.SaveChangesAsync();

        return _orderTransactionMapper.MapFromSourceToDestination(client);
    }

    private void ValidateTransactionPayload(OrderTransactionAddDto payload)
    {
        if (!EnumHelper.IsValidEnumValue(typeof(TransactionType), payload.TransactionType))
        {
            throw new ValidationExeption("Transaction type cannot be identified!");
        }
        if (payload.Amount <= 0)
        {
            throw new ValidationExeption("Transaction Amount must be a positive value!");
        }
    }

    public async Task<PagedList<OrderTransactionDto>> GetByPage(int orderId, int pageNumber, int pageSize)
    {

        PagedList<OrderTransaction> pagedList = await _unitOfWork.OrderTransactionRepository.FilterAsync(
            new Paging(pageNumber, pageSize),
            (transaction) => transaction.OrderId == orderId,
            new Sorting(nameof(OrderTransaction.Id), SortingDirection.DESC)
            );

        // check if no data returned then return no data founds

        var result = _orderTransactionMapper.MapFromSourceToDestination(pagedList);

        return result;
    }
}
