namespace Printpress.Application;

public record PurchaseInvoiceDto
{
    public Guid Id { get; init; }
    public string InvoiceNumber { get; init; }
    public DateTime InvoiceDate { get; init; }
    public string SupplierName { get; init; }
    public decimal TotalAmount { get; init; }
    public string AttachmentFilePath { get; init; }
    public List<PurchaseInvoiceLineDto> Lines { get; init; }
}
