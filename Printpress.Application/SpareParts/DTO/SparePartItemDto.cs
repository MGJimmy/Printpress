namespace Printpress.Application;

public record SparePartItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int? PacksPerCarton { get; init; }
    public int? UnitsPerPack { get; init; }
    public int StockQuantity { get; init; }
    public bool HasTransactions { get; init; }
}
