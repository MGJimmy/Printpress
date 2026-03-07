using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application
{
    public class ItemDetailsUpsertDTO : TrackedDTO
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemDetailsKeyEnum Key { get; set; }

        public string Value { get; set; }
    }
}
