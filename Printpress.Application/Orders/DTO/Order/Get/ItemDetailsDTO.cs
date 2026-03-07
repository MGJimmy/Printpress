using Printpress.Domain;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class ItemDetailsDTO
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemDetailsKeyEnum Key { get; set; }

        public string Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
