namespace Printpress.Application;

public class InventoryItemReportData
{
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public int ExpectedProductionWastePercent { get; set; }
}
