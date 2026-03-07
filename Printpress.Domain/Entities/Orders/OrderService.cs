
namespace Printpress.Domain
{
    public class OrderService : Entity ,ISoftDelete
    {
        public Guid OrderId { get; set; }
        public Guid ServiceId { get; set; }
        public decimal? Price { get; set; }
        public bool IsDeleted { get; set; }


        public virtual Order Order { get; set; }
        public virtual Service Service { get; set; }
    }
}
