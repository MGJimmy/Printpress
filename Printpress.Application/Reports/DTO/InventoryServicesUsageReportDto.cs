namespace Printpress.Application;

public class InventoryServicesUsageReportDto
{
    public List<InventoryItemUsageRowDto> InventoryItems { get; set; }
    public int TotalCartonsIn { get; set; }
    public int TotalUnitsIn { get; set; }
    public int TotalCartonsOut { get; set; }
    public int TotalUnitsOut { get; set; }
    public int TotalPeriodNetCartons { get; set; }
    public int TotalPeriodNetUnits { get; set; }
    public int TotalCurrentStockCartons { get; set; }
    public int TotalCurrentStockUnits { get; set; }

    public List<ServiceUsageRowDto> Services { get; set; }
    public int TotalOrders { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalPaperUsed { get; set; }
}
