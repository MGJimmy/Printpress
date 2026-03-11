namespace Printpress.Application;

public class ServiceUsageRowDto
{
    public string ServiceName { get; set; }
    public int OrderCount { get; set; }
    public int ItemCount { get; set; }
    public decimal PaperUsed { get; set; }
}
