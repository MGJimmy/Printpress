namespace Printpress.Application;

public interface IPurchaseInvoiceService
{
    Task<PurchaseInvoiceDto> CreateAsync(PurchaseInvoiceCreateDto payload, string userId);
    Task<InventoryPurchaseInvoiceListDto> GetAllAsync(
        int? categoryId,
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided);
    Task<InventoryPurchaseInvoiceListItemDto> GetByIdAsync(Guid id);
    Task VoidAsync(Guid id, string reason, string userId);
}
