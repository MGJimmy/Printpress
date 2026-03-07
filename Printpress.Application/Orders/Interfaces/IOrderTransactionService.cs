namespace Printpress.Application;

public interface IOrderTransactionService
{
    Task<OrderTransactionDto> AddAsync(OrderTransactionAddDto payload, string userId);
    Task<PagedList<OrderTransactionDto>> GetByPage(Guid orderId, int pageNumber, int pageSize);

}