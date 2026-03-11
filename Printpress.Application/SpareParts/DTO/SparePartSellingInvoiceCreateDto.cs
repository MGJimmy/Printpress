namespace Printpress.Application;

public class SparePartSellingInvoiceCreateDto
{
    public string ClientName { get; set; }
    public DateTime InvoiceDate { get; set; }
    public List<SparePartSellingInvoiceLineCreateDto> Lines { get; set; }
}

public class SparePartSellingInvoiceLineCreateDto
{
    public Guid SparePartItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
