
namespace Printpress.Domain
{
    public class OrderGroupService : Entity,ISoftDelete
    {
        public Guid OrderGroupId { get; set; }
        public Guid ServiceId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual OrderGroup OrderGroup { get; set; }
        public virtual Service Service { get; set; }
    }
}
