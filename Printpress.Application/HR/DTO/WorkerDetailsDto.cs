using Printpress.Domain;

namespace Printpress.Application;

public class WorkerDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Notes { get; set; }
    public SalaryType SalaryType { get; set; }
    public decimal? MonthlySalary { get; set; }
    public decimal? DailySalary { get; set; }
    public bool IsActive { get; set; }
    public List<WorkerSalaryTransactionDto> Transactions { get; set; }
    public List<WorkerProductionDto> Productions { get; set; }
    public WorkerTransactionsSummaryDto Stats { get; set; }
}
