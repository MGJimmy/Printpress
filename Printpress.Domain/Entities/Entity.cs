using Printpress.Domain.Enums;
using Printpress.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printpress.Domain.Entities;

public class Entity : ITrackedEntity 
{
    [NotMapped]
    public TrackingState State { get; set; }
}
