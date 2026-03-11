namespace Printpress.Application;

public record ServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal? Price { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public string ServiceCategoryCode { get; set; }
    public string ServiceCategoryName { get; set; }
    public Guid? InventoryItemId { get; set; }
    public string InventoryItemName { get; set; }
}
