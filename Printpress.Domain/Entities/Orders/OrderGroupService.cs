using Printpress.Domain.Interfaces;

namespace Printpress.Domain.Entities
{
    public class OrderGroupService : Entity,ISoftDelete
    {
        public int Id { get; set; }
        public int OrderGroupId { get; set; }
        public int ServiceId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual OrderGroup OrderGroup { get; set; }
        public virtual Service Service { get; set; }
    }
}
