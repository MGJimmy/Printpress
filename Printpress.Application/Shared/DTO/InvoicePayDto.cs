namespace Printpress.Application;

public class InvoicePayDto
{
    public decimal Amount { get; set; }
    public string Note { get; set; }
}

public class InvoicePaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
    public bool IsVoided { get; set; }
}
