using Printpress.Domain;

namespace Printpress.Application;

internal class InventoryItemMapper : BaseMapper<InventoryItem, InventoryItemDto>
{
    public override InventoryItemDto MapFromSourceToDestination(InventoryItem source)
    {
        return new InventoryItemDto
        {
            Id = source.Id,
            Name = source.Name,
            InventoryItemCategory = source.InventoryItemCategory,
            PacksPerCarton = source.PacksPerCarton,
            UnitsPerPack = source.UnitsPerPack,
            ExpectedPurchaseLossPercent = source.ExpectedPurchaseLossPercent,
            ExpectedProductionWastePercent = source.ExpectedProductionWastePercent
        };
    }

    public override InventoryItem MapFromDestinationToSource(InventoryItemDto dto)
        => throw new NotImplementedException();

    public InventoryItem MapFromAddDto(InventoryItemAddDto dto)
    {
        return new InventoryItem
        {
            Name = dto.Name,
            InventoryItemCategory = EnumHelper.MapStringToEnum<InventoryItemCategoryEnum>(dto.InventoryItemCategory),
            PacksPerCarton = dto.PacksPerCarton,
            UnitsPerPack = dto.UnitsPerPack,
            ExpectedPurchaseLossPercent = dto.ExpectedPurchaseLossPercent,
            ExpectedProductionWastePercent = dto.ExpectedProductionWastePercent
        };
    }

    public InventoryItem MapFromUpdateDto(int id, InventoryItemUpdateDto dto)
    {
        return new InventoryItem
        {
            Id = id,
            Name = dto.Name,
            InventoryItemCategory = EnumHelper.MapStringToEnum<InventoryItemCategoryEnum>(dto.InventoryItemCategory),
            PacksPerCarton = dto.PacksPerCarton,
            UnitsPerPack = dto.UnitsPerPack,
            ExpectedPurchaseLossPercent = dto.ExpectedPurchaseLossPercent,
            ExpectedProductionWastePercent = dto.ExpectedProductionWastePercent
        };
    }
}
