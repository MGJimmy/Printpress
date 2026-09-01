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
        bool? isVoided,
        bool? hasRemaining,
        bool? isGoodsReceived);
    Task<InventoryPurchaseInvoiceListItemDto> GetByIdAsync(Guid id);
    Task PayAsync(Guid id, InvoicePayDto payload, string userId);
    Task ReceiveGoodsAsync(Guid id, string userId);
    Task VoidAsync(Guid id, string reason, string userId);
}
