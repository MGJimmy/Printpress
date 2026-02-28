namespace Printpress.Application;

public class InventoryItemUpdateDto
{
    public string Name { get; set; }
    public string InventoryItemCategory { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public int ExpectedPurchaseLossPercent { get; set; }
    public int ExpectedProductionWastePercent { get; set; }
}
