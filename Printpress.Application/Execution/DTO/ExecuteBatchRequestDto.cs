namespace Printpress.Application;

public class ExecuteBatchRequestDto
{
    public Guid WorkerId { get; set; }
    public List<Guid> ServiceCategoryIds { get; set; }
    public DateTime ExecutionDate { get; set; }
    public string Notes { get; set; }
}

public class ExecuteItemBatchRequestDto : ExecuteBatchRequestDto
{
    public Guid OrderItemId { get; set; }
}

public class ExecuteGroupBatchRequestDto : ExecuteBatchRequestDto
{
    public Guid GroupId { get; set; }
}
