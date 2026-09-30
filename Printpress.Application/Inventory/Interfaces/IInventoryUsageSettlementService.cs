using Printpress.Domain;

namespace Printpress.Application;

public interface IInventoryUsageSettlementService
{
    Task CreateAsync(InventoryUsageSettlementCreateDto payload, string userId);

    Task<InventoryUsageSettlementListDto> GetAllAsync(
        int? categoryId,
        Guid? itemId,
        InventoryUsageSettlementType? type,
        DateTime? dateFrom,
        DateTime? dateToExclusive);
}
