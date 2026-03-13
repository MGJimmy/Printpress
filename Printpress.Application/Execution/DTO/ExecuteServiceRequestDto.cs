namespace Printpress.Application;

public class ExecuteServiceRequestDto
{
    public Guid OrderItemId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public DateTime ExecutionDate { get; set; }
    public string Notes { get; set; }
    public List<WorkerExecutionRowDto> Workers { get; set; }
}
