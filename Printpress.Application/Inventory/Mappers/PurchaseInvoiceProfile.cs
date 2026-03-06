using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

public class PurchaseInvoiceProfile : Profile
{
    public PurchaseInvoiceProfile()
    {
        CreateMap<PurchaseInvoice, PurchaseInvoiceDto>()
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.PurchaseInvoiceLines));

        CreateMap<PurchaseInvoiceLine, PurchaseInvoiceLineDto>();
    }
}
