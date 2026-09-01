namespace Printpress.Application;

public interface ISparePartPurchaseInvoiceService
{
    Task CreateAsync(SparePartPurchaseInvoiceCreateDto payload, string userId);
    Task<SparePartPurchaseInvoiceListDto> GetAllAsync(
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided);
    Task<SparePartPurchaseInvoiceListItemDto> GetByIdAsync(Guid id);
    Task VoidAsync(Guid id, string reason, string userId);
}
