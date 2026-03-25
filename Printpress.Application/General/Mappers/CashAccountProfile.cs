using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class CashAccountProfile : Profile
{
    public CashAccountProfile()
    {
        CreateMap<CashAccount, CashAccountDto>();
    }
}
