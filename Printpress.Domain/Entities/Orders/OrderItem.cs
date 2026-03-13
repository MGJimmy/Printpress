
namespace Printpress.Domain
{
    public class OrderItem : Entity , ISoftDelete
    {
        public string Name { get; set; }
        public Guid OrderGroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public OrderItemStatus OrderItemStatus { get; set; }

        public virtual OrderGroup OrderGroup { get; set; }
        public virtual List<OrderItemDetails> Details { get; set; }
    }
}
