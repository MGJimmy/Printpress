using Printpress.Domain.Interfaces;

namespace Printpress.Domain.Entities
{
    public class Item : Entity , ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderGroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public virtual OrderGroup OrderGroup { get; set; }
        public virtual List<ItemDetails> Details { get; set; }
    }
}
