namespace Printpress.Application;

public class ZeroOrderReportRowDto
{
    public Guid OrderId { get; set; }
    public string OrderName { get; set; }
    public string ClientName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public int ServiceCount { get; set; }
    public int ItemCount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ZeroOrdersReportDto
{
    public List<ZeroOrderReportRowDto> Orders { get; set; } = [];
    public int OrderCount { get; set; }
    public int ItemCount { get; set; }
}
