using Printpress.Domain.Enums;

namespace Printpress.Application;

public enum ObjectState
{
    Unchanged = 1,
    Added = 2,
    Updated = 3,
    Deleted = 4
}

public static class ObjectStateExtensions
{
    public static TrackingState MapToTrackingState(this ObjectState state)
    {
        return state switch
        {
            ObjectState.Unchanged => TrackingState.Unchanged,
            ObjectState.Added => TrackingState.Added,
            ObjectState.Updated => TrackingState.Modified,
            ObjectState.Deleted => TrackingState.Modified,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
 
}
