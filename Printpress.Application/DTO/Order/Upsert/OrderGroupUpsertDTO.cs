using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderGroupUpsertDTO : IObjectState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<OrderGroupServiceUpsertDTO> OrderGroupServices { get; set; }
        public List<ItemUpsertDTO> Items { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ObjectState ObjectState { get; set; }
    }
}
