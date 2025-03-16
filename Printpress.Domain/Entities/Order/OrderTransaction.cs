using Printpress.Domain.Enums;

namespace Printpress.Domain.Entities
{
    public class OrderTransaction : Entity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }


        public virtual Order Order { get; set; }
    }
}
