namespace Printpress.Application;

public class InventoryPurchaseLineRowDto
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string SupplierName { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class InventoryPurchaseReportDto
{
    public List<InventoryPurchaseLineRowDto> Lines { get; set; } = [];
    public int InvoiceCount { get; set; }
    public int LineCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
}
