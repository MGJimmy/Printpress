namespace Printpress.Application;

public class PurchaseInvoiceCreateDto
{
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string SupplierName { get; set; }
    public string AttachmentFilePath { get; set; }
    public List<PurchaseInvoiceLineCreateDto> Lines { get; set; }
}
