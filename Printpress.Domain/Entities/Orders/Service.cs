
namespace Printpress.Domain
{
    public class Service : Entity
    {
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public Guid? InventoryItemId { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ServiceCategory ServiceCategory { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }
    }
}
