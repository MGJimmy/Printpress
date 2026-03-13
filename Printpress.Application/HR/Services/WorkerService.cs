using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class WorkerService(
    IUnitOfWork _unitOfWork,
    IValidator<WorkerCreateDto> _createValidator,
    IValidator<WorkerUpdateDto> _updateValidator,
    IGuidGenerator _guidGenerator) : IWorkerService
{
    public async Task<List<WorkerDto>> GetAllAsync()
    {
        var workers = await _unitOfWork.WorkerRepository.AllAsync();
        return workers.Select(MapToDto).ToList();
    }

    public async Task<WorkerDetailsDto> GetDetailsAsync(Guid id, DateTime? productionDateFrom, DateTime? productionDateTo)
    {
        var worker = await _unitOfWork.WorkerRepository.FindAsync(id);
        if (worker is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        var transactions = _unitOfWork.WorkerSalaryTransactionRepository
            .Filter(t => t.WorkerId == id, nameof(WorkerSalaryTransaction.PayrollPeriod))
            .OrderByDescending(t => t.TransactionDate)
            .Select(MapTransactionToDto)
            .ToList();

        var productionQuery = _unitOfWork.WorkerProductionRepository
            .Filter(p => p.WorkerId == id,
                nameof(ItemServiceExecution.ServiceCategory),
                $"{nameof(ItemServiceExecution.OrderItem)}.{nameof(OrderItem.OrderGroup)}");

        if (productionDateFrom.HasValue)
            productionQuery = productionQuery.Where(p => p.ExecutionDate >= productionDateFrom.Value);
        if (productionDateTo.HasValue)
            productionQuery = productionQuery.Where(p => p.ExecutionDate <= productionDateTo.Value);

        var productions = productionQuery
            .OrderByDescending(p => p.ExecutionDate)
            .Select(MapProductionToDto)
            .ToList();

        var stats = CalculateStats(worker, transactions);

        return new WorkerDetailsDto
        {
            Id = worker.Id,
            Name = worker.Name,
            PhoneNumber = worker.PhoneNumber,
            Address = worker.Address,
            Notes = worker.Notes,
            SalaryType = worker.SalaryType,
            MonthlySalary = worker.MonthlySalary,
            DailySalary = worker.DailySalary,
            IsActive = worker.IsActive,
            Transactions = transactions,
            Productions = productions,
            Stats = stats
        };
    }

    public async Task<WorkerDto> CreateAsync(WorkerCreateDto payload, string userId)
    {
        var validationResult = await _createValidator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var worker = new Worker
        {
            Id = _guidGenerator.NewGuid(),
            Name = payload.Name,
            PhoneNumber = payload.PhoneNumber,
            Address = payload.Address,
            Notes = payload.Notes,
            SalaryType = payload.SalaryType,
            MonthlySalary = payload.SalaryType == SalaryType.Monthly ? payload.MonthlySalary : null,
            DailySalary = payload.SalaryType == SalaryType.Daily ? payload.DailySalary : null,
            IsActive = true
        };

        await _unitOfWork.WorkerRepository.AddAsync(worker);
        await _unitOfWork.SaveChangesAsync(userId);

        return MapToDto(worker);
    }

    public async Task<WorkerDto> UpdateAsync(WorkerUpdateDto payload, string userId)
    {
        var validationResult = await _updateValidator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var worker = await _unitOfWork.WorkerRepository.FindAsync(payload.Id);
        if (worker is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.Id));

        worker.Name = payload.Name;
        worker.PhoneNumber = payload.PhoneNumber;
        worker.Address = payload.Address;
        worker.Notes = payload.Notes;
        worker.SalaryType = payload.SalaryType;
        worker.MonthlySalary = payload.SalaryType == SalaryType.Monthly ? payload.MonthlySalary : null;
        worker.DailySalary = payload.SalaryType == SalaryType.Daily ? payload.DailySalary : null;

        _unitOfWork.WorkerRepository.Update(worker);
        await _unitOfWork.SaveChangesAsync(userId);

        return MapToDto(worker);
    }

    public async Task DeactivateAsync(Guid id, string userId)
    {
        var worker = await _unitOfWork.WorkerRepository.FindAsync(id);
        if (worker is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        if (!worker.IsActive)
            throw new ValidationExeption("العامل غير نشط بالفعل");

        worker.IsActive = false;

        _unitOfWork.WorkerRepository.Update(worker);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static WorkerDto MapToDto(Worker w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        PhoneNumber = w.PhoneNumber,
        Address = w.Address,
        Notes = w.Notes,
        SalaryType = w.SalaryType,
        MonthlySalary = w.MonthlySalary,
        DailySalary = w.DailySalary,
        IsActive = w.IsActive
    };

    private static WorkerSalaryTransactionDto MapTransactionToDto(WorkerSalaryTransaction t) => new()
    {
        Id = t.Id,
        WorkerName = t.Worker?.Name ?? string.Empty,
        TransactionType = t.TransactionType,
        Amount = t.Amount,
        TransactionDate = t.TransactionDate,
        Note = t.Note,
        PayrollPeriodName = t.PayrollPeriod?.Name ?? string.Empty
    };

    private static WorkerProductionDto MapProductionToDto(ItemServiceExecution p) => new()
    {
        Id = p.Id,
        ProductionDate = p.ExecutionDate,
        ServiceCategoryName = p.ServiceCategory?.Name ?? string.Empty,
        OrderName = p.OrderItem?.OrderGroup?.Name ?? string.Empty,
        Quantity = p.Quantity,
        Notes = p.Notes
    };

    private static WorkerSummaryStatsDto CalculateStats(Worker worker, List<WorkerSalaryTransactionDto> transactions)
    {
        var now = DateTime.UtcNow;
        var firstOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var thisMonthTransactions = transactions
            .Where(t => t.TransactionDate >= firstOfMonth)
            .ToList();

        var totalAdvances = thisMonthTransactions
            .Where(t => t.TransactionType == SalaryTransactionType.SalaryAdvance)
            .Sum(t => t.Amount);

        var totalPaid = thisMonthTransactions
            .Where(t => t.TransactionType != SalaryTransactionType.SalaryAdvance)
            .Sum(t => t.Amount);

        decimal? remaining = null;
        if (worker.SalaryType == SalaryType.Monthly && worker.MonthlySalary.HasValue)
            remaining = worker.MonthlySalary.Value - totalPaid;

        return new WorkerSummaryStatsDto
        {
            TotalAdvancesThisMonth = totalAdvances,
            TotalPaidThisMonth = totalPaid,
            RemainingThisMonth = remaining
        };
    }
}
