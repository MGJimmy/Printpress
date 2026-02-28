
namespace Printpress.Application
{
    public class ItemDTO : TrackedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<ItemDetailsDTO> Details { get; set; }
    }
}
