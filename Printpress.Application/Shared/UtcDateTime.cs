namespace Printpress.Application;

public static class UtcDateTime
{
    public static DateTime AsUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

    public static DateTime? AsUtc(DateTime? value) =>
        value is null ? null : AsUtc(value.Value);

    public static DateTime? StartOfDay(DateOnly? date) =>
        date is null
            ? null
            : DateTime.SpecifyKind(date.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

    public static DateTime? ExclusiveEnd(DateOnly? date) =>
        date is null
            ? null
            : DateTime.SpecifyKind(date.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
}
