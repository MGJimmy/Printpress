namespace Printpress.Application;

public class CashFlowReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? CashAccountId { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal Net { get; set; }
    public List<CashFlowBucketDto> ByDay { get; set; } = [];
    public List<CashFlowBucketDto> ByMonth { get; set; } = [];
}

public class CashFlowBucketDto
{
    public string Key { get; set; }
    public string Label { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal Net { get; set; }
    public int TransactionCount { get; set; }
}
