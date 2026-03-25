using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class CashTransactionDto
{
    public Guid Id { get; set; }
    public Guid CashAccountId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashTransactionType Type { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashTransactionCategory Category { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashTransactionReferenceType? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
