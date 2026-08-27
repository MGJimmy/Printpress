namespace Printpress.Application;

public class InventoryStockOutRowDto
{
    public Guid Id { get; set; }
    public DateTime MovementDate { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }
    public int Quantity { get; set; }
    public Guid? WorkerId { get; set; }
    public string WorkerName { get; set; }
    public string Notes { get; set; }
}

public class InventoryStockOutReportDto
{
    public List<InventoryStockOutRowDto> Rows { get; set; } = [];
    public int MovementCount { get; set; }
    public int TotalCartons { get; set; }
    public int ItemCount { get; set; }
    public int WorkerCount { get; set; }
}
