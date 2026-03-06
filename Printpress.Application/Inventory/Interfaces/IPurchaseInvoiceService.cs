namespace Printpress.Application;

public interface IPurchaseInvoiceService
{
    Task<PurchaseInvoiceDto> CreateAsync(PurchaseInvoiceCreateDto payload, string userId);
}
