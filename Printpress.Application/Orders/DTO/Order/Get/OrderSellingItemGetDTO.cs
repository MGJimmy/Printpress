using Printpress.Domain;

namespace Printpress.Application;

public class OrderSellingItemGetDTO : TrackedDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OrderId { get; set; }
    public Guid? InventoryItemId { get; set; }
    public bool IsInventoryItem { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? InventoryItemName { get; set; }
}
