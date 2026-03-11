namespace Printpress.Application;

public class OrderInventoryItemsReportDto
{
    public string ItemCategory { get; set; }
    public string ItemName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public decimal CartonsIn { get; set; }
    public int UnitsIn { get; set; }
    public decimal CartonsOut { get; set; }
    public int UnitsOut { get; set; }
    public decimal PaperUsedUnits { get; set; }
    public decimal ExpectedWaste { get; set; }
    public decimal Difference { get; set; }
}
