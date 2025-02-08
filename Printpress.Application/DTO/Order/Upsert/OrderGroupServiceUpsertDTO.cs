using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderGroupServiceUpsertDTO : IObjectState
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ObjectState ObjectState { get; set; }
    }
}
