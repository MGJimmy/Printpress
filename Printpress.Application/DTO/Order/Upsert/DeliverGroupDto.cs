namespace Printpress.Application;

public class DeliverGroupDto
{
    public int Id { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string DeliveredFrom { get; set; }
    public string DeliveredTo { get; set; }
    public string DeliveryNotes { get; set; }

}
