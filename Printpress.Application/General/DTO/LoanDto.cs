namespace Printpress.Application;

public class LoanListDto
{
    public Guid Id { get; set; }
    public int LoanNumber { get; set; }
    public Guid LenderId { get; set; }
    public string LenderName { get; set; }
    public decimal Principal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Remaining { get; set; }
    public DateTime OccurredAt { get; set; }
    public bool IsVoided { get; set; }
    public bool IsClosed { get; set; }
}

public class LoanDto : LoanListDto
{
    public string Notes { get; set; }
    public Guid CashAccountId { get; set; }
    public string CashAccountName { get; set; }
    public string VoidReason { get; set; }
    public DateTime? VoidedAt { get; set; }
    public string VoidedByName { get; set; }
    public List<InvoicePaymentDto> Payments { get; set; } = [];
}

public class LoanCreateDto
{
    public Guid LenderId { get; set; }
    public decimal Principal { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid CashAccountId { get; set; }
    public string Notes { get; set; }
}

public class LoanPayDto
{
    public decimal Amount { get; set; }
    public DateTime? OccurredAt { get; set; }
    public string Note { get; set; }
}

public class LoanVoidPaymentDto
{
    public Guid PaymentId { get; set; }
    public string Reason { get; set; }
}
