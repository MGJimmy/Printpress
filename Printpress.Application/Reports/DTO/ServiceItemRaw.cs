namespace Printpress.Application;

public class ServiceItemRaw
{
    public Guid ServiceId { get; set; }
    public int Quantity { get; set; }
    public string PagesValue { get; set; }
    public string FacesValue { get; set; }
    public bool IsCover { get; set; }
}
