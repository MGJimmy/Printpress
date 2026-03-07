namespace Printpress.Application;

public class PurchaseInvoiceLineCreateDto
{
    public int InventoryItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
