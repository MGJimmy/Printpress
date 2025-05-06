using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Printpress.Domain;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    public class TrackedDTO : ITrackedEntity
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrackingState ObjectState { get; set; }
    }
}
