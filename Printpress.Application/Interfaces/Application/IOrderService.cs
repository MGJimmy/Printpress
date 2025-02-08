namespace Printpress.Application;

public interface IOrderService
{
    Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize);

    Task InsertOrder(OrderUpsertDto order);
}
