using Printpress.Domain;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public class OrderSellingItemUpsertDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? InventoryItemId { get; set; }
    public bool IsInventoryItem { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TrackingState ObjectState { get; set; }
}
