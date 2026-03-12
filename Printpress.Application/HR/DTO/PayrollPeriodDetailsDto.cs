namespace Printpress.Application;

public class PayrollPeriodDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<WorkerSalaryTransactionDto> Transactions { get; set; }
}
