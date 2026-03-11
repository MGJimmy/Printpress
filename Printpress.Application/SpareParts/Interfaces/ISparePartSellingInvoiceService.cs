namespace Printpress.Application;

public interface ISparePartSellingInvoiceService
{
    Task<SparePartSellingInvoiceDto> CreateAsync(SparePartSellingInvoiceCreateDto payload, string userId);
}
