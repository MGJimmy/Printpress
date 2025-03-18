using System.Text.Json.Serialization;

namespace Printpress.Application
{
    public class OrderTransactionAddDto
    {
        public int OrderId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }

        public string? Note { get; set; }
    }
}
