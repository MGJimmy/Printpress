namespace Printpress.Application;

public class InventoryItemUsageRowDto
{
    public string ItemCategory { get; set; }
    public string ItemName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public int CartonsIn { get; set; }
    public int UnitsIn { get; set; }
    public int CartonsOut { get; set; }
    public int UnitsOut { get; set; }
    public int PeriodNetCartons { get; set; }
    public int PeriodNetUnits { get; set; }
    public int CurrentStockCartons { get; set; }
    public int CurrentStockUnits { get; set; }
    public int ExpectedProductionWastePercent { get; set; }
}
