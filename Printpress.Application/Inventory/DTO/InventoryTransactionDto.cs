using Printpress.Domain;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public record InventoryTransactionDto
{
    public Guid Id { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryTransactionType InventoryTransactionType { get; init; }

    public int Quantity { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryTransactionReferenceType ReferenceType { get; init; }

    public Guid ReferenceId { get; init; }
    public string Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public string InventoryItemName { get; init; }
    public Guid? WorkerId { get; init; }
    public string? WorkerName { get; init; }
}
