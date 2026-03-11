namespace Printpress.Application;

public record SparePartSellingInvoiceDto
{
    public Guid Id { get; init; }
    public int InvoiceNumber { get; init; }
    public string ClientName { get; init; }
    public DateTime InvoiceDate { get; init; }
    public decimal TotalAmount { get; init; }
}
