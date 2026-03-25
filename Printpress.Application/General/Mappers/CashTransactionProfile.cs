using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class CashTransactionProfile : Profile
{
    public CashTransactionProfile()
    {
        CreateMap<CashTransaction, CashTransactionDto>();
    }
}
