using Printpress.Domain;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderGroupDTO : TrackedDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveredFrom { get; set; }
        public string DeliveredTo { get; set; }
        public string DeliveryNotes { get; set; }
      
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GroupStatusEnum Status { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GroupExecutionType ExecutionType { get; set; }
        public List<OrderGroupServiceDTO> OrderGroupServices { get; set; }
        public List<ItemDTO> Items { get; set; }
    }
}
