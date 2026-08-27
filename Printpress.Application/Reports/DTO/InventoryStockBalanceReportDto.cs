namespace Printpress.Application;

public class InventoryStockBalanceRowDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public bool IsActive { get; set; }
    public int OpeningCartons { get; set; }
    public int OpeningUnits { get; set; }
    public int PeriodInCartons { get; set; }
    public int PeriodInUnits { get; set; }
    public int PeriodOutCartons { get; set; }
    public int PeriodOutUnits { get; set; }
    public int ClosingCartons { get; set; }
    public int ClosingUnits { get; set; }
}

public class InventoryStockBalanceReportDto
{
    public List<InventoryStockBalanceRowDto> Rows { get; set; } = [];
    public int ItemCount { get; set; }
    public int TotalOpeningCartons { get; set; }
    public int TotalPeriodInCartons { get; set; }
    public int TotalPeriodOutCartons { get; set; }
    public int TotalClosingCartons { get; set; }
    public int TotalClosingUnits { get; set; }
}
