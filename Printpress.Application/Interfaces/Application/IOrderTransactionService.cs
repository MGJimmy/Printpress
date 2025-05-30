namespace Printpress.Application;

public interface IOrderTransactionService
{
    Task<OrderTransactionDto> AddAsync(OrderTransactionAddDto payload, string userId);
    Task<PagedList<OrderTransactionDto>> GetByPage(int orderId, int pageNumber, int pageSize);

}