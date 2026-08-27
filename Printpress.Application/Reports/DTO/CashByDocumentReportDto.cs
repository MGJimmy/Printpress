using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class CashByDocumentReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? CashAccountId { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public List<CashDocumentGroupDto> Documents { get; set; } = [];
}

public class CashDocumentGroupDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashTransactionReferenceType? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }
    public string ReferenceTypeName { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal Net { get; set; }
}
