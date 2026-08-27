namespace Printpress.Application;

public interface IInventoryMovementReportService
{
    Task<InventoryMovementReportDto> GetReportAsync(Guid inventoryItemId, DateTime? dateFrom, DateTime? dateToExclusive);
}
