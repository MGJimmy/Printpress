using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application;

public class OrderService(IUnitOfWork _IUnitOfWork, OrderMapper _OrderMapper) : IOrderService
{
    public async Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize)
    {
        string[] includes = { nameof(Order.Client) };

        var orders = await _IUnitOfWork.OrderRepository.AllAsync(
            new Paging(pageNumber, pageSize),
            new Sorting(nameof(Order.Id), SortingDirection.DESC),
            includes
        );

        return _OrderMapper.MapToOrderSummeryDto(orders);
    }

    public async Task InsertOrder(OrderDto orderDTO)
    {

        Order order = _OrderMapper.MapFromDestinationToSource(orderDTO);

        order.Status = OrderStatusEnum.New;

        await _IUnitOfWork.OrderRepository.AddAsync(order);

        await _IUnitOfWork.SaveChangesAsync();
    }
}
