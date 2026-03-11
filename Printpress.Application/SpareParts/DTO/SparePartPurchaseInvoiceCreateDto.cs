namespace Printpress.Application;

public class SparePartPurchaseInvoiceCreateDto
{
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string SupplierName { get; set; }
    public string AttachmentFilePath { get; set; }
    public List<SparePartPurchaseInvoiceLineCreateDto> Lines { get; set; }
}

public class SparePartPurchaseInvoiceLineCreateDto
{
    public Guid SparePartItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
