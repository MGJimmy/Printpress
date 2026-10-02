using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public enum OutstandingBalanceDirection
{
    Collect,
    Pay
}

public enum OutstandingBalanceKind
{
    Order,
    WorkerAdvance,
    PurchaseInvoice,
    SparePartPurchaseInvoice,
    Loan,
    MonthlySalary
}

public class OutstandingDocumentProjection
{
    public Guid Id { get; set; }
    public string PartyName { get; set; }
    public string DocumentLabel { get; set; }
    public DateTime Date { get; set; }
    public decimal? Total { get; set; }
    public decimal? Paid { get; set; }
    public decimal Remaining { get; set; }
}

public class OutstandingMonthlySalaryWorkerProjection
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal MonthlySalary { get; set; }
}

public class OutstandingSalaryTransactionProjection
{
    public Guid WorkerId { get; set; }
    public SalaryTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
}

public class OutstandingBalanceRowDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OutstandingBalanceDirection Direction { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OutstandingBalanceKind Type { get; set; }

    public string PartyName { get; set; }
    public string DocumentLabel { get; set; }
    public DateTime Date { get; set; }
    public decimal? Total { get; set; }
    public decimal? Paid { get; set; }
    public decimal Remaining { get; set; }
    public string Route { get; set; }
}

public class OutstandingBalancesReportDto
{
    public decimal TotalToCollect { get; set; }
    public decimal TotalToPay { get; set; }
    public List<OutstandingBalanceRowDto> Rows { get; set; } = [];
}
