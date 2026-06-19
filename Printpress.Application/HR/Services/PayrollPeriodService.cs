using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class PayrollPeriodService(
    IUnitOfWork _unitOfWork,
    IValidator<PayrollPeriodCreateDto> _validator,
    IGuidGenerator _guidGenerator) : IPayrollPeriodService
{
    public async Task<PagedList<PayrollPeriodDto>> GetAllAsync(Paging paging)
    {
        var sorting = new Sorting(nameof(PayrollPeriod.CreatedAt), SortingDirection.DESC);

        var periods = await _unitOfWork.PayrollPeriodRepository.AllAsync(paging, sorting);


        return new PagedList<PayrollPeriodDto>
        {
            PageNumber = paging.PageNumber,
            PageSize = paging.PageSize,
            TotalCount = periods.TotalCount,
            Items = periods.Items.Select(MapToDto).ToList()
        };
    
    }

    public async Task<List<PayrollPeriodDto>> GetOpenPeriodsAsync()
    {
        var sorting = new Sorting(nameof(PayrollPeriod.CreatedAt), SortingDirection.DESC);

        var periods = await _unitOfWork.PayrollPeriodRepository.FilterAsync(x => x.IsClosed == false);


        return periods.Select(MapToDto).ToList();

    }

    public async Task<PayrollPeriodDetailsDto> GetDetailsAsync(Guid id)
    {
        var period = await _unitOfWork.PayrollPeriodRepository.FindAsync(id);
        if (period is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        var transactions = _unitOfWork.WorkerSalaryTransactionRepository
            .Filter(t => t.PayrollPeriodId == id, nameof(WorkerSalaryTransaction.Worker))
            .Select(MapTransactionToDto)
            .ToList();

        return new PayrollPeriodDetailsDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            IsClosed = period.IsClosed,
            ClosedAt = period.IsClosed ? period.ClosedAt : null,
            Transactions = transactions
        };
    }

    public async Task<PayrollPeriodDto> CreateAsync(PayrollPeriodCreateDto payload, string userId)
    {
        var validationResult = await _validator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        ValidateNoOverlap(payload.StartDate, payload.EndDate);

        var period = new PayrollPeriod
        {
            Id = _guidGenerator.NewGuid(),
            Name = payload.Name,
            StartDate = payload.StartDate,
            EndDate = payload.EndDate,
            IsClosed = false
        };

        await _unitOfWork.PayrollPeriodRepository.AddAsync(period);
        await _unitOfWork.SaveChangesAsync(userId);

        return MapToDto(period);
    }

    public async Task CloseAsync(Guid id, string userId)
    {
        var period = await _unitOfWork.PayrollPeriodRepository.FindAsync(id);
        if (period is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        if (period.IsClosed)
            throw new ValidationExeption("دورة الرواتب مغلقة بالفعل");

        period.IsClosed = true;
        period.ClosedAt = DateTime.UtcNow;

        _unitOfWork.PayrollPeriodRepository.Update(period);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public bool IsPeriodClosed(Guid periodId)
        => _unitOfWork.PayrollPeriodRepository.Any(p => p.Id == periodId && p.IsClosed);

    // ── Private helpers ──────────────────────────────────────────────────────

    private void ValidateNoOverlap(DateTime startDate, DateTime endDate)
    {
        var hasOverlap = _unitOfWork.PayrollPeriodRepository.Any(p =>
            p.StartDate <= endDate && p.EndDate >= startDate);

        if (hasOverlap)
            throw new ValidationExeption(
                "الفترة الزمنية المحددة تتداخل مع دورة رواتب موجودة. يرجى اختيار فترة مختلفة");
    }

    private static PayrollPeriodDto MapToDto(PayrollPeriod p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        IsClosed = p.IsClosed,
        ClosedAt = p.IsClosed ? p.ClosedAt : null
    };

    private static WorkerSalaryTransactionDto MapTransactionToDto(WorkerSalaryTransaction t) => new()
    {
        Id = t.Id,
        WorkerName = t.Worker?.Name ?? string.Empty,
        TransactionType = t.TransactionType,
        Amount = t.Amount,
        TransactionDate = t.TransactionDate,
        Note = t.Note
    };
}