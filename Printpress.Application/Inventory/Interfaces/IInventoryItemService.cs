namespace Printpress.Application;

public interface IInventoryItemService
{
    Task<InventoryItemDto> AddAsync(InventoryItemAddDto payload, string userId);
    Task<InventoryItemDto> UpdateAsync(Guid id, InventoryItemUpdateDto payload, string userId);
    Task<InventoryItemDto> GetByIdAsync(Guid id);
    Task<PagedList<InventoryItemDto>> GetAllAsync(Paging paging);
    Task<List<InventoryItemDto>> GetByCategoryAsync(int categoryId);
    Task DeleteAsync(Guid id, string userId);
    Task DeactivateAsync(Guid id, string userId);
}
