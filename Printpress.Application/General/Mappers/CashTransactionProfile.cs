using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class CashTransactionProfile : Profile
{
    public CashTransactionProfile()
    {
        CreateMap<CashTransaction, CashTransactionDto>()
            .ForMember(d => d.CanVoid, o => o.MapFrom(s => CashAccountDomainService.CanVoidFromVault(s)));
    }
}
