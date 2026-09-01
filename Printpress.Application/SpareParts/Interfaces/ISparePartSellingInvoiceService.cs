namespace Printpress.Application;

public interface ISparePartSellingInvoiceService
{
    Task<SparePartSellingInvoiceDto> CreateAsync(SparePartSellingInvoiceCreateDto payload, string userId);
    Task<SparePartSellingInvoiceListDto> GetAllAsync(
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided);
    Task<SparePartSellingInvoiceListItemDto> GetByIdAsync(Guid id);
    Task VoidAsync(Guid id, string reason, string userId);
}
