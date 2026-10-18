namespace Printpress.Application;

public interface IWorkerAttendanceService
{
    Task CheckInAsync(Guid workerId, DateTime? at, string userId);
    Task CheckOutAsync(Guid workerId, DateTime? at, string userId);
    Task AddClosedAsync(Guid workerId, DateTime? checkInAt, DateTime? checkOutAt, string userId);
    Task VoidAsync(Guid workerId, Guid attendanceId, string reason, string userId);
    Task<WorkerAttendanceListDto> GetByWorkerAsync(Guid workerId, DateTime? dateFrom, DateTime? dateToExclusive);
}
