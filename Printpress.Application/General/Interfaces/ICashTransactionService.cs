namespace Printpress.Application;

public interface ICashTransactionService
{
    Task<PagedList<CashTransactionDto>> GetByCashAccountIdAsync(
        Guid cashAccountId,
        Paging paging,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? type,
        string? category);

    Task<CashTransactionDto> AddAsync(AddCashTransactionDto payload, string userId);

    Task VoidAsync(Guid transactionId, VoidCashTransactionDto payload, string userId);

    Task TransferAsync(TransferCashTransactionDto payload, string userId);

    Task<List<ExternalOrderDto>> GetExternalOrdersAsync();
}
