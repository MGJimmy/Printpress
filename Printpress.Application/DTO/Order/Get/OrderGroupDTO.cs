using Printpress.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderGroupDTO : TrackedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveredFrom { get; set; }
        public string DeliveredTo { get; set; }
        public string DeliveryNotes { get; set; }
      
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GroupStatusEnum Status { get; set; }
        public List<OrderGroupServiceDTO> OrderGroupServices { get; set; }
        public List<ItemDTO> Items { get; set; }
    }
}
