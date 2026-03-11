
namespace Printpress.Domain
{
    public class Service : Entity
    {
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public Guid? InventoryItemId { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }
    }
}
