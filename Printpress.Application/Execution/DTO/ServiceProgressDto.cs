namespace Printpress.Application;

public class ServiceProgressDto
{
    public Guid ServiceCategoryId { get; set; }
    public string ServiceCategoryName { get; set; }
    public int Executed { get; set; }
    public int Total { get; set; }
    public bool IsCompleted => Executed >= Total;
}
