using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryTransactionProfile : Profile
{
    public InventoryTransactionProfile()
    {
        CreateMap<InventoryTransaction, InventoryTransactionDto>();
    }
}
