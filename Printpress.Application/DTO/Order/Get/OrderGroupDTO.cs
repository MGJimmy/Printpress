using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderGroupDTO : ITrackingState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public List<OrderGroupServiceDTO> OrderGroupServices { get; set; }
        public List<ItemDTO> Items { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
