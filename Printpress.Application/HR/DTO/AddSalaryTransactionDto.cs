using Printpress.Domain;

namespace Printpress.Application;

public class AddSalaryTransactionDto
{
    public Guid WorkerId { get; set; }
    public Guid PayrollPeriodId { get; set; }
    public SalaryTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Note { get; set; }
}
