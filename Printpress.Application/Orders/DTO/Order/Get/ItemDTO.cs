
using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application
{
    public class ItemDTO : TrackedDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid GroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderItemStatus Status { get; set; }

        public bool HasExecutions { get; set; }
        public List<ItemDetailsDTO> Details { get; set; }
    }
}
