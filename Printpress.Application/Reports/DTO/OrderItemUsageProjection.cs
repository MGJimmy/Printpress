namespace Printpress.Application;

public class OrderItemUsageProjection
{
    public int Quantity { get; set; }
    public int NumberOfPages { get; set; }
    public int NumberOfPrintingFaces { get; set; }
    public bool IsCover { get; set; }
}
