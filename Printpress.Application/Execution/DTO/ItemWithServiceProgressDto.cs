namespace Printpress.Application;

public class ItemWithServiceProgressDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
    public List<ServiceProgressDto> ServiceProgresses { get; set; }
}
