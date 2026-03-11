namespace Printpress.Application;

public interface ISparePartPurchaseInvoiceService
{
    Task CreateAsync(SparePartPurchaseInvoiceCreateDto payload, string userId);
}
