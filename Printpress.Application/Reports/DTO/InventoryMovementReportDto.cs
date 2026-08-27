using Printpress.Domain;

namespace Printpress.Application;

public class InventoryMovementLineDto
{
    public Guid Id { get; set; }
    public DateTime MovementDate { get; set; }
    public string Type { get; set; }
    public int InQuantity { get; set; }
    public int OutQuantity { get; set; }
    public int RunningBalance { get; set; }
    public string ReferenceType { get; set; }
    public string WorkerName { get; set; }
    public string Notes { get; set; }
}

public class InventoryMovementReportDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string CategoryName { get; set; }
    public int OpeningBalance { get; set; }
    public int TotalIn { get; set; }
    public int TotalOut { get; set; }
    public int ClosingBalance { get; set; }
    public List<InventoryMovementLineDto> Lines { get; set; } = [];
}

public class InventoryMovementTxProjection
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public InventoryTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public InventoryTransactionReferenceType ReferenceType { get; set; }
    public string WorkerName { get; set; }
    public string Notes { get; set; }
}
