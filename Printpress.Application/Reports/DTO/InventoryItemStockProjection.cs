namespace Printpress.Application;

public class InventoryItemStockProjection
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public int ExpectedProductionWastePercent { get; set; }
    public int CartonsIn { get; set; }
    public int CartonsOut { get; set; }
    public int CurrentStockCartons { get; set; }
}
