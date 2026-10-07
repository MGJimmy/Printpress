namespace Printpress.Application;

public class InventoryConversionCreateDto
{
    public Guid InventoryItemId { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }
    public DateTime OccurredAt { get; set; }
}
