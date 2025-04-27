using Printpress.Domain.Enums;

namespace Printpress.Application
{
    internal interface ITrackingState
    {
        public TrackingState ObjectState { get; set; }
    }
}
