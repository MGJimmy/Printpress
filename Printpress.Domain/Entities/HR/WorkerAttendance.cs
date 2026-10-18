namespace Printpress.Domain;

public class WorkerAttendance : Entity
{
    public Guid WorkerId { get; private set; }
    public DateTime CheckInAt { get; private set; }
    public DateTime? CheckOutAt { get; private set; }
    public decimal? Hours { get; private set; }
    public bool IsVoided { get; private set; }
    public string VoidReason { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string VoidedBy { get; private set; }

    public virtual Worker Worker { get; private set; }

    private WorkerAttendance()
    {
    }

    public WorkerAttendance(Guid workerId, DateTime checkInAt)
    {
        if (workerId == Guid.Empty)
            throw new BusinessExceptions(LocalizationKeys.HR.WorkerRequired);

        if (checkInAt == default)
            throw new BusinessExceptions(LocalizationKeys.HR.TimeRequired);

        WorkerId = workerId;
        CheckInAt = checkInAt;
    }

    public void Close(DateTime checkOutAt)
    {
        if (checkOutAt == default)
            throw new BusinessExceptions(LocalizationKeys.HR.TimeRequired);

        if (checkOutAt <= CheckInAt)
            throw new BusinessExceptions(LocalizationKeys.HR.LeaveMustBeAfterArrival);

        if ((checkOutAt - CheckInAt) > TimeSpan.FromHours(24))
            throw new BusinessExceptions(LocalizationKeys.HR.StretchExceeds24Hours);

        CheckOutAt = checkOutAt;
        Hours = Math.Round((decimal)(checkOutAt - CheckInAt).TotalHours, 2, MidpointRounding.AwayFromZero);
    }

    public void MarkAsVoided(string reason, string userId)
    {
        if (IsVoided)
            throw new BusinessExceptions(LocalizationKeys.HR.AttendanceAlreadyVoided);

        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessExceptions(LocalizationKeys.Invoices.ReasonRequired);

        var trimmed = reason.Trim();
        if (trimmed.Length > 500)
            throw new BusinessExceptions(LocalizationKeys.HR.VoidReasonMaxLength);

        IsVoided = true;
        VoidReason = trimmed;
        VoidedAt = DateTime.UtcNow;
        VoidedBy = userId;
    }
}
