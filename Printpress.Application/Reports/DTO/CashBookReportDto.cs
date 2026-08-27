using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class CashBookReportDto
{
    public Guid? CashAccountId { get; set; }
    public string CashAccountName { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<CashBookAccountSummaryDto> AccountSummaries { get; set; } = [];
    public List<CashBookLineDto> Lines { get; set; } = [];
    public int TotalLineCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class CashBookAccountSummaryDto
{
    public Guid CashAccountId { get; set; }
    public string CashAccountName { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal ClosingBalance { get; set; }
}

public class CashBookLineDto
{
    public Guid Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public Guid CashAccountId { get; set; }
    public string CashAccountName { get; set; }
    public decimal InAmount { get; set; }
    public decimal OutAmount { get; set; }
    public decimal RunningBalance { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CashTransactionCategory Category { get; set; }

    public string Description { get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}
