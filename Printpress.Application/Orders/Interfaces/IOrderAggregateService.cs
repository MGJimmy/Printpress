namespace Printpress.Application;

public interface IOrderAggregateService
{
    Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize);

    Task InsertOrder(OrderUpsertDto order, string userId);

    Task<OrderDto> GetOrderDTOAsync(int orderId);
    Task UpdateOrder(int id, OrderUpsertDto orderDTO, string userId);

    Task<OrderMainDataDto> GetOrderMainDataAsync(int orderId);

    Task DeleteOrder(int id, string userId);
}
