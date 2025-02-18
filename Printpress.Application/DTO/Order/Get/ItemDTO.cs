using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class ItemDTO : IObjectState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ObjectState ObjectState { get; set; }
    }
}
