namespace Printpress.Application
{
    public class OrderTransactionAddDto
    {
        public int OrderId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }

    }
}
