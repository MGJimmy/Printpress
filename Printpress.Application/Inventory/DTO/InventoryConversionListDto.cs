using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryConversionListRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryConversionStatus Status { get; set; }

    public DateTime? CompletedAt { get; set; }
    public string CompletedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public bool IsVoided { get; set; }
    public string VoidReason { get; set; }
    public DateTime? VoidedAt { get; set; }
    public string VoidedBy { get; set; }
}

public class InventoryConversionListDto
{
    public List<InventoryConversionListRowDto> Rows { get; set; } = [];
    public int Count { get; set; }
    public int TotalQuantity { get; set; }
}
