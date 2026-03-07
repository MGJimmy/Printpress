
namespace Printpress.Domain
{
    public class OrderTransaction : Entity
    {
        public Guid OrderId { get; set; }
        public OrderTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }


        public virtual Order Order { get; set; }
    }
}
