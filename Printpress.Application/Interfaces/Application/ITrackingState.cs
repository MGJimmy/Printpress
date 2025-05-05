using Printpress.Domain.Enums;

namespace Printpress.Application
{
    public interface ITrackingState
    {
        public TrackingState ObjectState { get; set; }
    }
}
