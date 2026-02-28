namespace Printpress.Application;

public interface IInventoryItemService
{
    Task<InventoryItemDto> AddAsync(InventoryItemAddDto payload, string userId);
    Task<InventoryItemDto> UpdateAsync(int id, InventoryItemUpdateDto payload, string userId);
    Task<InventoryItemDto> GetByIdAsync(int id);
    Task<List<InventoryItemDto>> GetAllAsync();
    Task DeleteAsync(int id, string userId);
}
