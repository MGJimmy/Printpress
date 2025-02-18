namespace Printpress.Application;

public interface IOrderAggregateService
{
    Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize);

    Task InsertOrder(OrderUpsertDto order);
}
