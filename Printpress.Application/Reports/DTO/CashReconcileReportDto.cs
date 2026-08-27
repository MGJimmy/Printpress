namespace Printpress.Application;

public class CashReconcileReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int AccountCount { get; set; }
    public int MismatchCount { get; set; }
    public decimal TotalStoredBalance { get; set; }
    public decimal TotalComputedBalance { get; set; }
    public decimal TotalDifference { get; set; }
    public List<CashReconcileAccountDto> Accounts { get; set; } = [];
}

public class CashReconcileAccountDto
{
    public Guid CashAccountId { get; set; }
    public string CashAccountName { get; set; }
    public string AccountType { get; set; }
    public decimal StoredBalance { get; set; }
    public decimal ComputedBalance { get; set; }
    public decimal Difference { get; set; }
    public bool IsMatched { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal PeriodIn { get; set; }
    public decimal PeriodOut { get; set; }
    public decimal PeriodClosing { get; set; }
    public bool PeriodIdentityOk { get; set; }
}
