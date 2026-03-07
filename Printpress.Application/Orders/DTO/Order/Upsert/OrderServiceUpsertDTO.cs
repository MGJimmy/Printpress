using Printpress.Domain;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderServiceUpsertDTO
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public decimal? Price { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
