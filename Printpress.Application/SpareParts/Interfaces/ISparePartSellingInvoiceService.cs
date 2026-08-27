namespace Printpress.Application;

public interface ISparePartSellingInvoiceService
{
    Task<SparePartSellingInvoiceDto> CreateAsync(SparePartSellingInvoiceCreateDto payload, string userId);
    Task<SparePartSellingInvoiceListDto> GetAllAsync(Guid? itemId, DateTime? dateFrom, DateTime? dateToExclusive);
}
