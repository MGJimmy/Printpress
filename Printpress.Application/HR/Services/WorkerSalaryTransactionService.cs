using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class WorkerSalaryTransactionService(
    IUnitOfWork _unitOfWork,
    IValidator<AddSalaryTransactionDto> _validator,
    IGuidGenerator _guidGenerator,
    IWorkerTransactionCalculator workerTransactionCalculator) : IWorkerSalaryTransactionService
{
    public async Task<WorkerSalaryTransactionDto> AddAsync(AddSalaryTransactionDto payload, string userId)
    {
        var validationResult = await _validator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var worker = await _unitOfWork.WorkerRepository.FindAsync(payload.WorkerId);
        if (worker is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.WorkerId));

        if (!worker.IsActive)
            throw new ValidationExeption("لا يمكن إضافة حركة مالية لعامل غير نشط");

        var period = await _unitOfWork.PayrollPeriodRepository.FindAsync(payload.PayrollPeriodId);
        if (period is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.PayrollPeriodId));

        if (period.IsClosed)
            throw new ValidationExeption("لا يمكن إضافة حركة مالية في دورة رواتب مغلقة");


        ValidatePayment(payload, worker);

        var transaction =  worker.AddTransaction(
            _guidGenerator.NewGuid(),
            payload.PayrollPeriodId,
            payload.TransactionType,
            payload.Amount,
            payload.TransactionDate,
            payload.Note);


        _unitOfWork.WorkerRepository.Update(worker);
        await _unitOfWork.SaveChangesAsync(userId);

        return new WorkerSalaryTransactionDto
        {
            Id = transaction.Id,
            WorkerName = worker.Name,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            TransactionDate = transaction.TransactionDate,
            Note = transaction.Note,
            PayrollPeriodName = period.Name
        };
    }

    private void ValidatePayment(AddSalaryTransactionDto payload, Worker worker)
    {
        var thisMonthTransactions = _unitOfWork.WorkerSalaryTransactionRepository.GetThisMonthSalaryTransactions(payload.WorkerId);

        var transactionSummary = workerTransactionCalculator.Calculate(worker, thisMonthTransactions);

        if (payload.TransactionType == SalaryTransactionType.SalaryAdvancePayment
            && (worker.UnpaidAdvanceAmount - payload.Amount) < 0)
        {
            throw new ValidationExeption("لا يمكن رد قيمة سلفة اكبر من المتبقي من السلفة ");

        }

        if ((payload.TransactionType == SalaryTransactionType.Penalty 
            || payload.TransactionType == SalaryTransactionType.Salary)
            && transactionSummary.RemainingThisMonth - payload.Amount < 0)
        {
            throw new ValidationExeption("غير متبقى من راتب هذا الشهر مبلغ يمكن خصم القيمة المطلوبة منه");
        }
    }

    public async Task DeleteAsync(Guid transactionId, string userId)
    {
        var transaction = await _unitOfWork.WorkerSalaryTransactionRepository.FindAsync(transactionId);
        if (transaction is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(transactionId));

        var periodIsClosed = _unitOfWork.PayrollPeriodRepository
            .Any(p => p.Id == transaction.PayrollPeriodId && p.IsClosed);

        if (periodIsClosed)
            throw new ValidationExeption("لا يمكن حذف حركة مالية في دورة رواتب مغلقة");

        _unitOfWork.WorkerSalaryTransactionRepository.Remove(transaction);
        await _unitOfWork.SaveChangesAsync(userId);
    }
}
