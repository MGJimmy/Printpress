namespace Printpress.Application;

public interface IInventoryItemService
{
    Task<InventoryItemDto> AddAsync(InventoryItemAddDto payload, string userId);
    Task<InventoryItemDto> UpdateAsync(Guid id, InventoryItemUpdateDto payload, string userId);
    Task<InventoryItemDto> GetByIdAsync(Guid id);
    Task<PagedList<InventoryItemDto>> GetAllAsync(Paging paging);
    Task DeleteAsync(Guid id, string userId);
}
