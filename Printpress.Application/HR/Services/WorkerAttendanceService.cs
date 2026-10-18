using Printpress.Domain;

namespace Printpress.Application;

internal sealed class WorkerAttendanceService(
    IUnitOfWork _unitOfWork,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc) : IWorkerAttendanceService
{
    public async Task CheckInAsync(Guid workerId, DateTime? at, string userId)
    {
        var time = RequireTime(at);
        var worker = await LoadActiveWorkerAsync(workerId);
        var stretch = worker.CheckIn(time);
        stretch.Id = _guidGenerator.NewGuid();
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task CheckOutAsync(Guid workerId, DateTime? at, string userId)
    {
        var time = RequireTime(at);
        var worker = await LoadWorkerAsync(workerId);
        worker.CheckOut(time);
        _unitOfWork.WorkerRepository.Update(worker);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task AddClosedAsync(Guid workerId, DateTime? checkInAt, DateTime? checkOutAt, string userId)
    {
        var arrival = RequireTime(checkInAt);
        var leave = RequireTime(checkOutAt);
        var worker = await LoadActiveWorkerAsync(workerId);
        var stretch = worker.AddClosed(arrival, leave);
        stretch.Id = _guidGenerator.NewGuid();
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task VoidAsync(Guid workerId, Guid attendanceId, string reason, string userId)
    {
        reason = InvoiceVoidHelper.RequireReason(reason, _loc);
        var worker = await LoadWorkerAsync(workerId);

        var stretch = worker.Attendance.FirstOrDefault(a => a.Id == attendanceId)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.HR.AttendanceNotFound));

        stretch.MarkAsVoided(reason, userId);
        _unitOfWork.WorkerRepository.Update(worker);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task<WorkerAttendanceListDto> GetByWorkerAsync(
        Guid workerId, DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.HR.InvalidDateRange));

        var worker = await LoadWorkerAsync(workerId);

        var rows = worker.Attendance
            .Where(a => (dateFrom == null || a.CheckInAt >= dateFrom)
                && (dateToExclusive == null || a.CheckInAt < dateToExclusive))
            .OrderByDescending(a => a.CheckInAt)
            .Select(MapRow)
            .ToList();

        return new WorkerAttendanceListDto
        {
            WorkerId = worker.Id,
            IsCheckedIn = worker.Attendance.Any(a => !a.IsVoided && a.CheckOutAt is null),
            Rows = rows,
            TotalHours = rows.Where(r => !r.IsVoided && r.Hours is not null).Sum(r => r.Hours.Value)
        };
    }

    private DateTime RequireTime(DateTime? at)
    {
        if (at is null || at.Value == default)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.HR.TimeRequired));

        return UtcDateTime.AsUtc(at.Value);
    }

    private async Task<Worker> LoadActiveWorkerAsync(Guid workerId)
    {
        var worker = await LoadWorkerAsync(workerId);
        if (!worker.IsActive)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.HR.WorkerInactive));
        return worker;
    }

    private async Task<Worker> LoadWorkerAsync(Guid workerId)
    {
        if (workerId == Guid.Empty)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.HR.WorkerRequired));

        var worker = await _unitOfWork.WorkerRepository.FirstOrDefaultAsync(
            w => w.Id == workerId,
            track: true,
            nameof(Worker.Attendance));

        if (worker is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.HR.WorkerNotFound));

        return worker;
    }

    private static WorkerAttendanceRowDto MapRow(WorkerAttendance a) => new()
    {
        Id = a.Id,
        CheckInAt = a.CheckInAt,
        CheckOutAt = a.CheckOutAt,
        Hours = a.Hours,
        IsVoided = a.IsVoided,
        VoidReason = a.VoidReason,
        VoidedAt = a.VoidedAt
    };
}
