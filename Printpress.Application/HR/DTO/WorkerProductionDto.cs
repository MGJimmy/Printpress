namespace Printpress.Application;

public class WorkerProductionDto
{
    public Guid Id { get; set; }
    public DateTime ProductionDate { get; set; }
    public string ServiceCategoryName { get; set; }
    public string ItemName { get; set; }
    public string GroupName { get; set; }
    public Guid? OrderId { get; set; }
    public string OrderName { get; set; }
    public string ClientName { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }
}
