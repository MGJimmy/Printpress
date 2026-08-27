using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class WorkerSalaryTransactionService(
    IUnitOfWork _unitOfWork,
    IValidator<AddSalaryTransactionDto> _validator,
    IGuidGenerator _guidGenerator,
    IWorkerTransactionCalculator workerTransactionCalculator,
    ILocalizationService _loc,
    CashAccountDomainService _cashAccountDomainService) : IWorkerSalaryTransactionService
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

        var transaction = worker.AddTransaction(
            _guidGenerator.NewGuid(),
            payload.PayrollPeriodId,
            payload.TransactionType,
            payload.Amount,
            payload.TransactionDate,
            payload.Note);

        _unitOfWork.WorkerRepository.Update(worker);

        await AddCashTransaction(
            transaction.Id,
            payload.TransactionType,
            payload.Amount,
            worker.Name,
            payload.Note,
            payload.TransactionDate);

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

    private async Task AddCashTransaction(
        Guid salaryTransactionId,
        SalaryTransactionType salaryTransactionType,
        decimal amount,
        string workerName,
        string note,
        DateTime transactionDate)
    {
        // Penalty reduces remaining salary payable only. It is not cash in or out of the vault.
        if (salaryTransactionType == SalaryTransactionType.Penalty)
            return;

        var cashAccount = await _unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == CashAccountType.Main)
                            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        CashTransactionType cashTransactionType = GetCashTransactionType(salaryTransactionType);

        string description = _loc.Get(LocalizationKeys.CashAccounts.addSalaryTransactionDescription, salaryTransactionType.ToString(), workerName, note);

        _cashAccountDomainService.AddCashAccountTransaction(
            cashAccount,
            cashTransactionType,
            CashTransactionCategory.Salaries,
            CashTransactionReferenceType.WorkerSalaryTransaction,
            salaryTransactionId,
            amount,
            description,
            transactionDate);

        _unitOfWork.CashAccountRepository.Update(cashAccount);
    }

    private CashTransactionType GetCashTransactionType(SalaryTransactionType salaryTransactionType)
    {
        if (salaryTransactionType == SalaryTransactionType.SalaryAdvancePayment)
        {
            return CashTransactionType.In;
        }

        return CashTransactionType.Out;
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

        var cashTransaction = await _unitOfWork.CashTransactionRepository.FirstOrDefaultAsync(
            t => t.ReferenceType == CashTransactionReferenceType.WorkerSalaryTransaction
                 && t.ReferenceId == transaction.Id
                 && t.ReversesTransactionId == null);

        if (cashTransaction is not null && !cashTransaction.IsVoided)
        {
            var cashAccount = await _unitOfWork.CashAccountRepository.FindAsync(cashTransaction.CashAccountId)
                ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

            var description = _loc.Get(LocalizationKeys.CashAccounts.VoidDescription, cashTransaction.Description ?? string.Empty);
            if (description.Length > 500)
                description = description[..500];

            _cashAccountDomainService.Void(cashAccount, cashTransaction, description, DateTime.UtcNow);
            _unitOfWork.CashTransactionRepository.Update(cashTransaction);
            _unitOfWork.CashAccountRepository.Update(cashAccount);
        }

        _unitOfWork.WorkerSalaryTransactionRepository.Remove(transaction);
        await _unitOfWork.SaveChangesAsync(userId);
    }
}
