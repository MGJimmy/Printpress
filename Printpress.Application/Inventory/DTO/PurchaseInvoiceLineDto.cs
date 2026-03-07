namespace Printpress.Application;

public record PurchaseInvoiceLineDto
{
    public Guid Id { get; init; }
    public Guid InventoryItemId { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
