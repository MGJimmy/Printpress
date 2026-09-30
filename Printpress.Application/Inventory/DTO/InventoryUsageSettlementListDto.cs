using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryUsageSettlementListRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }
    public int Quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryUsageSettlementType SettlementType { get; set; }

    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
}

public class InventoryUsageSettlementListDto
{
    public List<InventoryUsageSettlementListRowDto> Rows { get; set; } = [];
    public int Count { get; set; }
    public int TotalQuantity { get; set; }
}
