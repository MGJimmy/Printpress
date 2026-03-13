namespace Printpress.Application;

public interface IWorkerSalaryTransactionService
{
    Task<WorkerSalaryTransactionDto> AddAsync(AddSalaryTransactionDto payload, string userId);
    Task DeleteAsync(Guid transactionId, string userId);
}
