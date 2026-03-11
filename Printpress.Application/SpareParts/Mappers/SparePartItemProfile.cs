using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class SparePartItemProfile : Profile
{
    public SparePartItemProfile()
    {
        CreateMap<SparePartItemAddDto, SparePartInventoryItem>();
        CreateMap<SparePartItemUpdateDto, SparePartInventoryItem>();
    }
}
