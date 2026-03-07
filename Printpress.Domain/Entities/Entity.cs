using System.ComponentModel.DataAnnotations.Schema;

namespace Printpress.Domain;

public abstract class Entity : IAuditableEntity, ITrackedEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    [NotMapped]
    public TrackingState ObjectState { get; set; }
}
