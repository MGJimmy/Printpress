namespace Printpress.Application;

public interface ISparePartPurchaseInvoiceService
{
    Task<Guid> CreateAsync(SparePartPurchaseInvoiceCreateDto payload, string userId);
    Task<SparePartPurchaseInvoiceListDto> GetAllAsync(
        Guid? itemId,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize,
        bool? isVoided,
        bool? hasRemaining,
        bool? isGoodsReceived);
    Task<SparePartPurchaseInvoiceListItemDto> GetByIdAsync(Guid id);
    Task PayAsync(Guid id, InvoicePayDto payload, string userId);
    Task ReceiveGoodsAsync(Guid id, string userId);
    Task VoidAsync(Guid id, string reason, string userId);
}
