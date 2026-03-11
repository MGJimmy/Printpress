using Printpress.Domain;

namespace Printpress.Application;

public interface ISparePartItemRepository : IGenericRepository<SparePartInventoryItem>
{
    Task<PagedList<SparePartItemDto>> GetAllWithStockQuantityAsync(Paging paging);
    Task<SparePartItemDto?> FindByIdWithStockQuantityAsync(Guid id);
}
