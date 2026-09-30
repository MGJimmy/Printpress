using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryUsageSettlementCreateDto
{
    public Guid InventoryItemId { get; set; }
    public int Quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryUsageSettlementType SettlementType { get; set; }

    public string Notes { get; set; }
    public DateTime OccurredAt { get; set; }
}
