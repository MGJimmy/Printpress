namespace Printpress.Application;

public interface IInventoryItemService
{
    Task<InventoryItemDto> AddAsync(InventoryItemAddDto payload, string userId);
    Task<InventoryItemDto> UpdateAsync(int id, InventoryItemUpdateDto payload, string userId);
    Task<InventoryItemDto> GetByIdAsync(int id);
    Task<PagedList<InventoryItemDto>> GetAllAsync(Paging paging);
    Task DeleteAsync(int id, string userId);
}
