
namespace Printpress.Domain
{
    public class Order : Entity , ISoftDelete
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalPaid { get; set; }
        public bool IsDeleted { get; set; }

        public OrderStatusEnum Status { get; set; }


        public virtual Client Client { get; set; }
        public virtual List<OrderTransaction> Transactions { get; set; }
        public virtual List<OrderGroup> OrderGroups { get; set; }
        public virtual List<OrderService> Services { get; set; }
    }
}
