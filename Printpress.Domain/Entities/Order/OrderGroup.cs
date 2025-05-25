using Printpress.Domain.Interfaces;
using Printpress.Enums;

namespace Printpress.Domain.Entities
{
    public class OrderGroup : Entity, ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryName { get; set; }
        public string ReceiverName { get; set; }
        public string DeliveryNotes { get; set; }
        public GroupStatusEnum Status { get; set; }
        public bool IsDeleted { get; set; }


        public virtual Order Order { get; set; }
        public virtual List<Item> Items { get; set; }
        public virtual List<OrderGroupService> OrderGroupServices { get; set; }
    }
}
