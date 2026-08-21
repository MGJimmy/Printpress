namespace Printpress.Application;

public class InventoryItemSelectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
}
