
namespace Printpress.Application;

public class OrderMainDataDto
{
    public string Name { get; set; }
    public string ClientName { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? TotalPaid { get; set; }
}
