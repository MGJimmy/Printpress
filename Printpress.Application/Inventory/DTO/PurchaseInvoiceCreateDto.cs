namespace Printpress.Application;

public class PurchaseInvoiceCreateDto
{
    public DateTime InvoiceDate { get; set; }
    public string SupplierName { get; set; }
    public string AttachmentFilePath { get; set; }
    public decimal? PaidNow { get; set; }
    public bool? ReceiveNow { get; set; }
    public List<PurchaseInvoiceLineCreateDto> Lines { get; set; }
}
