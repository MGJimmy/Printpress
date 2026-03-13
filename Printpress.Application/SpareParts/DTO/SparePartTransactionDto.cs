using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public record SparePartTransactionDto
{
    public Guid Id { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SparePartInventoryTransactionType InventoryTransactionType { get; init; }

    public int Quantity { get; init; }
    public string Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}
