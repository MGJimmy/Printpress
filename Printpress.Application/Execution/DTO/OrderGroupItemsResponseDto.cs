namespace Printpress.Application;

public class OrderGroupItemsResponseDto
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupStatus { get; set; }
    public string ExecutionType { get; set; }
    public List<ServiceProgressDto> GroupServices { get; set; }
    public List<ItemWithServiceProgressDto> Items { get; set; }
}
