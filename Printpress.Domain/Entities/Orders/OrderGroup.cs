
namespace Printpress.Domain
{
    public class OrderGroup : Entity, ISoftDelete
    {
        public string Name { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryName { get; set; }
        public string ReceiverName { get; set; }
        public string DeliveryNotes { get; set; }
        public GroupStatusEnum Status { get; set; }
        public bool IsDeleted { get; set; }


        public virtual Order Order { get; set; }
        public virtual List<OrderItem> Items { get; set; }
        public virtual List<OrderGroupService> OrderGroupServices { get; set; }
    }
}
