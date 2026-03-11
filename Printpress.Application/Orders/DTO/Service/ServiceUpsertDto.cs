namespace Printpress.Application;

public record ServiceUpsertDto
{
    public string Name { get; set; }
    public decimal? Price { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public Guid? InventoryItemId { get; set; }
}
