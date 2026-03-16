namespace Printpress.Application;

public interface IInventoryTransactionService
{
    Task<PagedList<InventoryTransactionDto>> GetByItemIdAsync(Guid itemId, Paging paging, DateTime? dateFrom, DateTime? dateTo, string transactionType);
    Task StockOutAsync(StockOutCreateDto payload, string userId);
    Task<PagedList<InventoryTransactionDto>> GetByWorkerIdAsync(Guid workerId, Paging paging, int? inventoryItemCategoryId, Guid? inventoryItemId, DateTime? dateFrom, DateTime? dateTo);
}
