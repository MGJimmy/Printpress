namespace Printpress.Application;

public class PurchaseInvoiceLineCreateDto
{
    public Guid InventoryItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
