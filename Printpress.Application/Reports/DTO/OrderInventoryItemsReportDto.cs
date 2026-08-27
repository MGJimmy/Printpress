namespace Printpress.Application;

public class OrderInventoryItemsReportDto
{
    public string ItemCategory { get; set; }
    public string ItemName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public int CartonsIn { get; set; }
    public int UnitsIn { get; set; }
    public int CartonsOut { get; set; }
    public int UnitsOut { get; set; }
    public decimal PaperUsedUnits { get; set; }
    public decimal ExpectedWaste { get; set; }
    public decimal Difference { get; set; }
    public int CurrentStockCartons { get; set; }
    public int CurrentStockUnits { get; set; }
    public int PeriodNetCartons { get; set; }
    public int PeriodNetUnits { get; set; }
}
