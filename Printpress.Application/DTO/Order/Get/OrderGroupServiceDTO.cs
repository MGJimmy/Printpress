using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderGroupServiceDTO : ITrackingState
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int OrderGroupId { get; set; }
        public string ServiceName { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
