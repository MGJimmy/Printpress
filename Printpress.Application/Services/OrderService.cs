using Printpress.Application.DTO.Order;
using Printpress.Application.Interfaces;
using Printpress.Application.Mappers;
using Printpress.Domain.Entities;

namespace Printpress.Application.Services;

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

}
