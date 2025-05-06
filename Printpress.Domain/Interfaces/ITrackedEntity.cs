using Printpress.Domain.Enums;

namespace Printpress.Domain;

public interface ITrackedEntity
{
    public TrackingState ObjectState { get; set; }
}
