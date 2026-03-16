namespace Printpress.Application;

public class StockOutCreateDto
{
    public Guid InventoryItemId { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }
    public Guid? WorkerId { get; set; }
}
