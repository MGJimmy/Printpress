using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class ItemUpsertDTO : ITrackingState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public List<ItemDetailsUpsertDTO> Details { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
