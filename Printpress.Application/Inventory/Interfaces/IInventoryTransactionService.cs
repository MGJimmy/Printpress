namespace Printpress.Application;

public interface IInventoryTransactionService
{
    Task<PagedList<InventoryTransactionDto>> GetByItemIdAsync(Guid itemId, Paging paging, DateTime? dateFrom, DateTime? dateTo, string transactionType);
    Task StockOutAsync(StockOutCreateDto payload, string userId);
}
