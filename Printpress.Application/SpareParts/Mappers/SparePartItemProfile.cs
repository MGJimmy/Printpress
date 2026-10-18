using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class SparePartItemProfile : Profile
{
    public SparePartItemProfile()
    {
        CreateMap<SparePartItemAddDto, SparePartInventoryItem>();
        CreateMap<SparePartItemUpdateDto, SparePartInventoryItem>();
        CreateMap<SparePartInventoryTransaction, SparePartTransactionDto>()
            .ForMember(dest => dest.ReferenceLabel, opt => opt.Ignore())
            .ForMember(dest => dest.ReferenceRoute, opt => opt.Ignore());
    }
}
