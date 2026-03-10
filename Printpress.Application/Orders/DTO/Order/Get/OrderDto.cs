using Printpress.Domain;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public class OrderDto : TrackedDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ClientName { get; set; }
    public Guid ClientId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatusEnum  Status { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? TotalPaid { get; set; }

    public List<OrderGroupDTO> OrderGroups { get; set; }
    public List<OrderServiceDTO> OrderServices { get; set; }
    public List<OrderSellingItemGetDTO> SellingItems { get; set; }
}
