using Printpress.Domain;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public record InventoryItemDto
{
    public int Id { get; init; }
    public string Name { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryItemCategoryEnum InventoryItemCategory { get; init; }

    public int? PacksPerCarton { get; init; }
    public int? UnitsPerPack { get; init; }
    public int ExpectedPurchaseLossPercent { get; init; }
    public int ExpectedProductionWastePercent { get; init; }
}
