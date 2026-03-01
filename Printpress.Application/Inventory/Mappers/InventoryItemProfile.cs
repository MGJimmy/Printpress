using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryItemProfile : Profile
{
    public InventoryItemProfile()
    {
        CreateMap<InventoryItem, InventoryItemDto>();

        CreateMap<InventoryItemAddDto, InventoryItem>()
            .ForMember(
                dest => dest.InventoryItemCategory,
                opt => opt.MapFrom(src => EnumHelper.MapStringToEnum<InventoryItemCategoryEnum>(src.InventoryItemCategory)));

        CreateMap<InventoryItemUpdateDto, InventoryItem>()
            .ForMember(
                dest => dest.InventoryItemCategory,
                opt => opt.MapFrom(src => EnumHelper.MapStringToEnum<InventoryItemCategoryEnum>(src.InventoryItemCategory)));
    }
}
