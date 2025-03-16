using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public record ServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? Price { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ServiceCategoryEnum ServiceCategory { get; set; }

}
