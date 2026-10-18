namespace Printpress.Application;

public class WorkerAttendanceTimeDto
{
    public DateTime? At { get; set; }
}

public class WorkerAttendanceClosedCreateDto
{
    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
}

public class WorkerAttendanceRowDto
{
    public Guid Id { get; set; }
    public DateTime CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
    public decimal? Hours { get; set; }
    public bool IsVoided { get; set; }
    public string VoidReason { get; set; }
    public DateTime? VoidedAt { get; set; }
}

public class WorkerAttendanceListDto
{
    public Guid WorkerId { get; set; }
    public bool IsCheckedIn { get; set; }
    public List<WorkerAttendanceRowDto> Rows { get; set; } = [];
    public decimal TotalHours { get; set; }
}
