namespace Printpress.Application
{
    public class OrderTransactionDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
