using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class CashMovementSummaryReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? CashAccountId { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal Net { get; set; }
    public int TransactionCount { get; set; }
    public List<CashMovementSliceDto> ByCategory { get; set; } = [];
    public List<CashMovementSliceDto> ByAccount { get; set; } = [];
}

public class CashMovementSliceDto
{
    public string Key { get; set; }
    public string Label { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashTransactionCategory? Category { get; set; }

    public Guid? CashAccountId { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal Net { get; set; }
    public int TransactionCount { get; set; }
}
