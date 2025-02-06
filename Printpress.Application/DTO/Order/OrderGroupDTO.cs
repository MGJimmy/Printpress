using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class OrderGroupDTO: IObjectState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public List<OrderGroupServiceDTO> OrderGroupServices { get; set; }
        public List<ItemDTO> Items { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
