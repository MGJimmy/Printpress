using System.Text.Json.Serialization;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    public class ItemDetailsUpsertDTO : TrackedDTO
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemDetailsKeyEnum Key { get; set; }

        public string Value { get; set; }
    }
}
