using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application
{
    public class OrderGroupUpsertDTO : TrackedDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GroupExecutionType ExecutionType { get; set; } = GroupExecutionType.Internal;
        public List<OrderGroupServiceUpsertDTO> OrderGroupServices { get; set; }
        public List<ItemUpsertDTO> Items { get; set; }
    }
}
