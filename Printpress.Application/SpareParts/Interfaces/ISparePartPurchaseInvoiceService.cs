namespace Printpress.Application;

public interface ISparePartPurchaseInvoiceService
{
    Task CreateAsync(SparePartPurchaseInvoiceCreateDto payload, string userId);
    Task<SparePartPurchaseInvoiceListDto> GetAllAsync(Guid? itemId, DateTime? dateFrom, DateTime? dateToExclusive);
}
