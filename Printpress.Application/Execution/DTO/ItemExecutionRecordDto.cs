namespace Printpress.Application;

public class ItemExecutionRecordDto
{
    public Guid Id { get; set; }
    public string WorkerName { get; set; }
    public string ServiceCategoryName { get; set; }
    public int Quantity { get; set; }
    public DateTime ExecutionDate { get; set; }
    public string Notes { get; set; }
}
