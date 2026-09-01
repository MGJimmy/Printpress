namespace Printpress.Application;

public class InventoryPurchaseInvoiceLineDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class InventoryPurchaseInvoiceListItemDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string SupplierName { get; set; }
    public decimal TotalAmount { get; set; }
    public string AttachmentFilePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVoided { get; set; }
    public string VoidReason { get; set; }
    public DateTime? VoidedAt { get; set; }
    public string VoidedBy { get; set; }
    public string VoidedByName { get; set; }
    public List<InventoryPurchaseInvoiceLineDto> Lines { get; set; } = [];
}

public class InventoryPurchaseInvoiceListDto
{
    public List<InventoryPurchaseInvoiceListItemDto> Invoices { get; set; } = [];
    public int InvoiceCount { get; set; }
    public int LineCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
}
