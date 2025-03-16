using Printpress.Domain.Enums;

namespace Printpress.Domain.Entities
{
    public class OrderGroup : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryName { get; set; }
        public string ReceiverName { get; set; }
        public OrderStatusEnum Status { get; set; }


        public virtual Order Order { get; set; }
        public virtual List<Item> Items { get; set; }
        public virtual List<OrderGroupService> Services { get; set; }
    }
}
