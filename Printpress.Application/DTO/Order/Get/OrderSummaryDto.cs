using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public record OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderName { get; set; }
    public string ClientName { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatusEnum OrderStatus { get; set; }

}


