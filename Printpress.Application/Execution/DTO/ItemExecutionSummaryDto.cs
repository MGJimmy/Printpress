namespace Printpress.Application;

public class ItemExecutionSummaryDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
    public Guid GroupId { get; set; }
    public List<ServiceProgressDto> ServiceProgresses { get; set; }
}
