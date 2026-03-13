namespace Printpress.Application;

public class WorkerSummaryStatsDto
{
    public decimal TotalAdvancesThisMonth { get; set; }
    public decimal TotalPaidThisMonth { get; set; }
    public decimal? RemainingThisMonth { get; set; }
}
