using Printpress.Domain.Entities;
using Printpress.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Printpress.Application;

public class OrderDto : IObjectState
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int ClientId { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? TotalPaid { get; set; }
    public ObjectState ObjectState { get; set; }

    public List<OrderGroup> OrderGroups { get; set; } //DTO
}
