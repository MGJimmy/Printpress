using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryTransactionProfile : Profile
{
    public InventoryTransactionProfile()
    {
        CreateMap<InventoryTransaction, InventoryTransactionDto>()
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.Name : null))
            .ForMember(dest => dest.InventoryItemName, opt => opt.MapFrom(src => src.InventoryItem != null ? src.InventoryItem.Name : null));
    }
}
