namespace Printpress.Application;

public interface IPurchaseInvoiceService
{
    Task<PurchaseInvoiceDto> CreateAsync(PurchaseInvoiceCreateDto payload, string userId);
    Task<InventoryPurchaseInvoiceListDto> GetAllAsync(
        int? categoryId, Guid? itemId, DateTime? dateFrom, DateTime? dateToExclusive);
}
