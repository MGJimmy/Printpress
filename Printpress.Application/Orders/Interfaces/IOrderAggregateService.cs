namespace Printpress.Application;

public interface IOrderAggregateService
{
    Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize);

    Task InsertOrder(OrderUpsertDto order, string userId);

    Task<OrderDto> GetOrderDTOAsync(Guid orderId);
    Task UpdateOrder(Guid id, OrderUpsertDto orderDTO, string userId);

    Task<OrderMainDataDto> GetOrderMainDataAsync(Guid orderId);

    Task DeleteOrder(Guid id, string userId);
}
