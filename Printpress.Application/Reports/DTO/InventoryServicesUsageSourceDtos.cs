namespace Printpress.Application;

public class ServiceUsageExecuteRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public string ServiceName { get; set; }
    public string OrderName { get; set; }
    public string WorkerName { get; set; }
    public int Quantity { get; set; }
    public decimal PaperUnits { get; set; }
    public string Notes { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}

public class ServiceUsageOrderRowDto
{
    public Guid OrderId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string ServiceName { get; set; }
    public string OrderName { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}

public class ServiceExecuteProjection
{
    public Guid Id { get; set; }
    public DateTime ExecutionDate { get; set; }
    public string ServiceName { get; set; }
    public Guid OrderId { get; set; }
    public string OrderName { get; set; }
    public Guid OrderGroupId { get; set; }
    public Guid OrderItemId { get; set; }
    public string WorkerName { get; set; }
    public int Quantity { get; set; }
    public int NumberOfPages { get; set; }
    public int NumberOfPrintingFaces { get; set; }
    public bool IsCover { get; set; }
    public string Notes { get; set; }
}

public class ServiceOrderProjection
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string OrderName { get; set; }
    public string ServiceName { get; set; }
}
