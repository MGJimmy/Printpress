using System.Text.Json.Serialization;
using Printpress.Domain.Enums;

namespace Printpress.Application;

public record ServiceUpsertDto
{
    public string Name { get; set; }
    public decimal? Price { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ServiceCategoryEnum ServiceCategory { get; set; }

}
