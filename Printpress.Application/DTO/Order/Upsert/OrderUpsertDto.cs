using Printpress.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public class OrderUpsertDto : ITrackingState
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int ClientId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TrackingState ObjectState { get; set; }
    public List<OrderGroupUpsertDTO> OrderGroups { get; set; }
    public List<OrderServiceUpsertDTO> OrderServices { get; set; }
}
