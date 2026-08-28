using AutoMapper;
using Printpress.Domain;
using Printpress.Domain.Entities.Inventory.DomainServices;

namespace Printpress.Application;

public class InventoryItemProfile : Profile
{
    public InventoryItemProfile()
    {
        CreateMap<InventoryItem, InventoryItemDto>()
                        .ForMember(dest => dest.StockQuantity,
                            opt => opt.MapFrom(src => InventoryCalculatorDS.CalculateStockQuantity(src.InventoryTransactions)))
                        .ForMember(dest => dest.TotalInQuantity,
                            opt => opt.MapFrom(src => InventoryCalculatorDS.CalculateInQuantity(src.InventoryTransactions)))
                        .ForMember(dest => dest.TotalOutQuantity,
                            opt => opt.MapFrom(src => InventoryCalculatorDS.CalculateOutQuantity(src.InventoryTransactions)));

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
