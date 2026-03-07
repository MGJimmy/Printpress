
namespace Printpress.Application
{
    public class ItemDTO : TrackedDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid GroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<ItemDetailsDTO> Details { get; set; }
    }
}
