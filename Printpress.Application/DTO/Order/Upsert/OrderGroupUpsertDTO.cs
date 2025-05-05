namespace Printpress.Application
{
    public class OrderGroupUpsertDTO : TrackedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<OrderGroupServiceUpsertDTO> OrderGroupServices { get; set; }
        public List<ItemUpsertDTO> Items { get; set; }
    }
}
