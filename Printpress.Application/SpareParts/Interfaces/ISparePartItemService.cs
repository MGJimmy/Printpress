namespace Printpress.Application;

public interface ISparePartItemService
{
    Task<PagedList<SparePartItemDto>> GetAllAsync(Paging paging);
    Task<List<SparePartItemDto>> GetAllForSelectionAsync();
    Task<SparePartItemDto> GetByIdAsync(Guid id);
    Task<SparePartItemDto> AddAsync(SparePartItemAddDto payload, string userId);
    Task<SparePartItemDto> UpdateAsync(Guid id, SparePartItemUpdateDto payload, string userId);
    Task DeleteAsync(Guid id, string userId);
}
