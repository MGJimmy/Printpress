using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderServiceUpsertDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public decimal? Price { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
