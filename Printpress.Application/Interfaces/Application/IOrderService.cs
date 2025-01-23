using Printpress.Application.DTO.Order;
using System.Threading.Tasks;

namespace Printpress.Application.Interfaces;

public interface IOrderService
{
    Task<PagedList<OrderSummaryDto>> GetOrderSummaryListAsync(int pageNumber, int pageSize);
}
