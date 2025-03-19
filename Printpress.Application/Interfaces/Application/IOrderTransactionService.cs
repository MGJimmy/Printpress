namespace Printpress.Application;

public interface IOrderTransactionService
{
    Task<OrderTransactionDto> AddAsync(OrderTransactionAddDto payload);
    Task<PagedList<OrderTransactionDto>> GetByPage(int orderId, int pageNumber, int pageSize);

}