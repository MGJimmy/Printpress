namespace Printpress.Application;

public interface IInventoryTransactionService
{
    Task<PagedList<InventoryTransactionDto>> GetByItemIdAsync(Guid itemId, Paging paging);
}
