using System.Linq.Expressions;
using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class WorkerService(
    IUnitOfWork _unitOfWork,
    IValidator<WorkerCreateDto> _createValidator,
    IValidator<WorkerUpdateDto> _updateValidator,
    IGuidGenerator _guidGenerator,
    IWorkerTransactionCalculator _workerSalaryCalculator) : IWorkerService
{
    public async Task<PagedList<WorkerDto>> GetAllAsync(Paging paging)
    {
        var sorting = new Sorting(nameof(Worker.CreatedAt), SortingDirection.DESC);
        var workerPageList = await _unitOfWork.WorkerRepository.AllAsync(paging, sorting);
        return new PagedList<WorkerDto>
        {
            PageNumber = paging.PageNumber,
            PageSize = paging.PageSize,
            TotalCount = workerPageList.TotalCount,
            Items = workerPageList.Items.Select(MapToDto).ToList()
        };
    }

    public async Task<List<WorkerDto>> GetActiveAsync()
    {
        var workers = await _unitOfWork.WorkerRepository.AllAsync();
        return workers.Where(x => x.IsActive).Select(MapToDto).ToList();
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

        var thisMonthtransactions = GetThisMonthTransactions(id);
        
        
        var transactionSummary = _workerSalaryCalculator.Calculate(worker, thisMonthtransactions);

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
            Stats = MapToWorkerTransactionSummaryDTO(transactionSummary)
        };
    }


    public async Task<PagedList<WorkerProductionDto>> GetWorkerProduction(Guid id, Paging paging, DateTime? productionDateFrom, DateTime? productionDateTo)
    {
        Expression<Func<ItemServiceExecution, bool>> filter =
         p =>
        p.WorkerId == id
        && (!productionDateFrom.HasValue || p.ExecutionDate >= productionDateFrom.Value)
        && (!productionDateTo.HasValue || p.ExecutionDate <= productionDateTo.Value);

        Sorting sorting = new Sorting(nameof(ItemServiceExecution.ExecutionDate), SortingDirection.DESC);

        var productionPageList = await _unitOfWork.WorkerProductionRepository
            .FilterAsync(
                paging,
                filter,
                sorting,
                nameof(ItemServiceExecution.ServiceCategory),
                $"{nameof(ItemServiceExecution.OrderItem)}.{nameof(OrderItem.OrderGroup)}",
                $"{nameof(ItemServiceExecution.OrderItem)}.{nameof(OrderItem.OrderGroup)}.{nameof(OrderGroup.Order)}");

        var productions = productionPageList.Items
            .Select(MapProductionToDto)
            .ToList();

        return new PagedList<WorkerProductionDto>
        {
            Items = productions,
            TotalCount = productionPageList.TotalCount,
            PageNumber = productionPageList.PageNumber,
            PageSize = productionPageList.PageSize
        };
    }




    private IEnumerable<WorkerSalaryTransaction> GetThisMonthTransactions(Guid workerId)
    {
        var now = DateTime.UtcNow;
        var firstOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

       return _unitOfWork.WorkerSalaryTransactionRepository
            .Filter(t => t.WorkerId == workerId && t.TransactionDate >= firstOfMonth);
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

    public async Task activateAsync(Guid id, string userId)
    {
        var worker = await _unitOfWork.WorkerRepository.FindAsync(id);
        if (worker is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        if (worker.IsActive)
            throw new ValidationExeption("العامل نشط بالفعل");

        worker.IsActive = true;

        _unitOfWork.WorkerRepository.Update(worker);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    #region Mappers
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
        OrderName = p.OrderItem?.OrderGroup?.Order?.Name ?? string.Empty,
        GroupName = p.OrderItem?.OrderGroup?.Name ?? string.Empty,
        ItemName = p.OrderItem?.Name ?? string.Empty,
        Quantity = p.Quantity,
        Notes = p.Notes
    };

    private WorkerTransactionsSummaryDto MapToWorkerTransactionSummaryDTO(WorkerTransactionSummary transactionSummary)
    {
        return new WorkerTransactionsSummaryDto
        {
            RemainingAdvances = transactionSummary.RemainingAdvances,
            TotalPaidThisMonth = transactionSummary.TotalPaidThisMonth,
            RemainingThisMonth = transactionSummary.RemainingThisMonth,
            TotalBounsThisMonth = transactionSummary.TotalBounsThisMonth,
            TotalPenaltyThisMonth = transactionSummary.TotalPenaltyThisMonth
        };
    }

    #endregion


}
