namespace Printpress.Application;

public class WorkerTransactionsSummaryDto
{
    public decimal RemainingAdvances { get; set; }
    public decimal TotalPaidThisMonth { get; set; }
    public decimal TotalBounsThisMonth { get; set; }
    public decimal TotalPenaltyThisMonth { get; set; }
    public decimal? RemainingThisMonth { get; set; }
}
