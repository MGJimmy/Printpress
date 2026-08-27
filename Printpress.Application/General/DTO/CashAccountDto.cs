using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class CashAccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashAccountType Type { get; set; }
}
