namespace Printpress.Application;

public record ServiceCategoryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public bool RequireInventoryItem { get; set; }
    public int? InventoryItemCategoryId { get; set; }
    public string InventoryItemCategoryName { get; set; }
}
