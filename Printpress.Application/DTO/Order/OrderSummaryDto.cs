using Printpress.Domain.Enums;

namespace Printpress.Application;

public record OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderName { get; set; }
    public string ClientName { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; } 
    public DateTime CreatedAt { get; set; }
    public OrderStatusEnum OrderStatus { get; set; }

}


