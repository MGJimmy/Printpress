namespace Printpress.Application;

public interface IOrderInventoryItemsReportService
{
    Task<OrderInventoryItemsReportDto> GetReportAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
 
}
