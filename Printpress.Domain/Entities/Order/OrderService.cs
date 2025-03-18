using Printpress.Domain.Interfaces;

namespace Printpress.Domain.Entities
{
    public class OrderService : Entity ,ISoftDelete
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public decimal? Price { get; set; }
        public bool IsDeleted { get; set; }


        public virtual Order Order { get; set; }
        public virtual Service Service { get; set; }
    }
}
