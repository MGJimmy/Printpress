namespace Printpress.Application
{
    public class OrderTransactionDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
