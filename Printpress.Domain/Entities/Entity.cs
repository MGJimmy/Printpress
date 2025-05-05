using Printpress.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printpress.Domain.Entities;

public class Entity : ITrackedEntity
{
    [NotMapped]
    public TrackingState ObjectState { get; set; }
}
