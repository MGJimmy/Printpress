namespace Printpress.Application
{
    public class ItemUpsertDTO : TrackedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public List<ItemDetailsUpsertDTO> Details { get; set; }
    }
}
