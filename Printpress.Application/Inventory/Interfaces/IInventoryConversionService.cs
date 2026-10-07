using Printpress.Domain;

namespace Printpress.Application;

public interface IInventoryConversionService
{
    Task CreateAsync(InventoryConversionCreateDto payload, string userId);
    Task CompleteAsync(Guid id, string userId);
    Task VoidAsync(Guid id, string reason, string userId);

    Task<InventoryConversionListDto> GetAllAsync(
        int? categoryId,
        Guid? itemId,
        InventoryConversionStatus? status,
        bool? isVoided,
        DateTime? dateFrom,
        DateTime? dateToExclusive);
}
