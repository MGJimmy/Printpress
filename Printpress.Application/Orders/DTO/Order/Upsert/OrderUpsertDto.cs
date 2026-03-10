using System.ComponentModel.DataAnnotations;

namespace Printpress.Application;

public class OrderUpsertDto : TrackedDTO
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public Guid ClientId { get; set; }

    public List<OrderGroupUpsertDTO> OrderGroups { get; set; }
    public List<OrderServiceUpsertDTO> OrderServices { get; set; }
    public List<OrderSellingItemUpsertDTO> SellingItems { get; set; }
}
