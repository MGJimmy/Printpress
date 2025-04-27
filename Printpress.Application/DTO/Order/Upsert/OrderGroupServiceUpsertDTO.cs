using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderGroupServiceUpsertDTO : ITrackingState
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
