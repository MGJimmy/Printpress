using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryTransactionListRowDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryTransactionType InventoryTransactionType { get; set; }

    public int Quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryTransactionReferenceType ReferenceType { get; set; }

    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
    public Guid? WorkerId { get; set; }
    public string WorkerName { get; set; }
    public string Notes { get; set; }
}

public class InventoryTransactionListDto
{
    public List<InventoryTransactionListRowDto> Rows { get; set; } = [];
    public int MovementCount { get; set; }
    public int ItemCount { get; set; }
    public int TotalInQuantity { get; set; }
    public int TotalOutQuantity { get; set; }
}
