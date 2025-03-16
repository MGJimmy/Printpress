using Printpress.Domain.Enums;

namespace Printpress.Domain.Entities
{
    public class Order : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ClientId { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalPaid { get; set; }
        public OrderStatusEnum Status { get; set; }


        public virtual Client Client { get; set; }
        public virtual List<OrderTransaction> Transactions { get; set; }
        public virtual List<OrderGroup> OrderGroups { get; set; }
        public virtual List<OrderService> Services { get; set; }
    }
}
