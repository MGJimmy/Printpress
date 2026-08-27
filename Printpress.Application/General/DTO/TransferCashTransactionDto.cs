namespace Printpress.Application;

public class TransferCashTransactionDto
{
    public Guid FromCashAccountId { get; set; }
    public Guid ToCashAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
}
