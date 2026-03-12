using Printpress.Domain;

namespace Printpress.Application;

public class WorkerSalaryTransactionDto
{
    public Guid Id { get; set; }
    public string WorkerName { get; set; }
    public SalaryTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Note { get; set; }
}
