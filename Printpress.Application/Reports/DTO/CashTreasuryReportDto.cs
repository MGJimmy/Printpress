namespace Printpress.Application;

public class CashTreasuryReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal TotalStoredBalance { get; set; }
    public List<CashTreasuryAccountDto> Accounts { get; set; } = [];
    public List<CashTreasuryMovementDto> LargestIn { get; set; } = [];
    public List<CashTreasuryMovementDto> LargestOut { get; set; } = [];
    public List<CashTransferRegisterRowDto> Transfers { get; set; } = [];
}

public class CashTreasuryAccountDto
{
    public Guid CashAccountId { get; set; }
    public string CashAccountName { get; set; }
    public string AccountType { get; set; }
    public decimal StoredBalance { get; set; }
}

public class CashTreasuryMovementDto
{
    public Guid Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public string CashAccountName { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
}

public class CashTransferRegisterRowDto
{
    public Guid TransferId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string FromAccountName { get; set; }
    public string ToAccountName { get; set; }
    public string Description { get; set; }
    public bool IsComplete { get; set; }
}
