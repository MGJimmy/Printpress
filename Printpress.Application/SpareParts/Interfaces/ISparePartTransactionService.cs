namespace Printpress.Application;

public interface ISparePartTransactionService
{
    Task<PagedList<SparePartTransactionDto>> GetByItemIdAsync(
        Guid itemId,
        Paging paging,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        string transactionType);
}
