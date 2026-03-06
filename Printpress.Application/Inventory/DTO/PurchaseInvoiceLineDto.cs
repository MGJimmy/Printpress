namespace Printpress.Application;

public record PurchaseInvoiceLineDto
{
    public int Id { get; init; }
    public int InventoryItemId { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
