using System.ComponentModel.DataAnnotations;

namespace Printpress.Application;

public class OrderUpsertDto : TrackedDTO
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int ClientId { get; set; }

    public List<OrderGroupUpsertDTO> OrderGroups { get; set; }
    public List<OrderServiceUpsertDTO> OrderServices { get; set; }
}
