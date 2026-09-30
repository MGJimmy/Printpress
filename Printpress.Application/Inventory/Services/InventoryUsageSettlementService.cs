using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class InventoryUsageSettlementService(
    IUnitOfWork _unitOfWork,
    IValidator<InventoryUsageSettlementCreateDto> _createValidator,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc) : IInventoryUsageSettlementService
{
    public async Task CreateAsync(InventoryUsageSettlementCreateDto payload, string userId)
    {
        var validationResult = await _createValidator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var item = await _unitOfWork.InventoryItemRepository.FindAsync(payload.InventoryItemId);
        if (item is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Inventory.ItemNotFound));

        var settlement = new InventoryUsageSettlement(
            payload.InventoryItemId,
            payload.Quantity,
            payload.SettlementType,
            payload.Notes,
            UtcDateTime.AsUtc(payload.OccurredAt));
        settlement.Id = _guidGenerator.NewGuid();

        await _unitOfWork.InventoryUsageSettlementRepository.AddAsync(settlement);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task<InventoryUsageSettlementListDto> GetAllAsync(
        int? categoryId,
        Guid? itemId,
        InventoryUsageSettlementType? type,
        DateTime? dateFrom,
        DateTime? dateToExclusive)
    {
        InvoiceVoidHelper.EnsureDateRange(dateFrom, dateToExclusive, _loc);

        var settlements = (await _unitOfWork.InventoryUsageSettlementRepository.FilterAsync(
                s => (dateFrom == null || s.OccurredAt >= dateFrom)
                    && (dateToExclusive == null || s.OccurredAt < dateToExclusive)
                    && (itemId == null || s.InventoryItemId == itemId)
                    && (categoryId == null || s.InventoryItem.InventoryItemCategoryId == categoryId)
                    && (type == null || s.SettlementType == type),
                nameof(InventoryUsageSettlement.InventoryItem),
                $"{nameof(InventoryUsageSettlement.InventoryItem)}.{nameof(InventoryItem.InventoryItemCategory_LKP)}"))
            .OrderByDescending(s => s.OccurredAt)
            .ToList();

        var rows = settlements.Select(s => new InventoryUsageSettlementListRowDto
        {
            Id = s.Id,
            OccurredAt = s.OccurredAt,
            ItemId = s.InventoryItemId,
            ItemName = s.InventoryItem?.Name ?? "—",
            CategoryName = s.InventoryItem?.InventoryItemCategory_LKP?.Name ?? "—",
            Quantity = s.Quantity,
            SettlementType = s.SettlementType,
            Notes = s.Notes,
            CreatedAt = s.CreatedAt,
            CreatedBy = s.CreatedBy
        }).ToList();

        return new InventoryUsageSettlementListDto
        {
            Rows = rows,
            Count = rows.Count,
            TotalQuantity = rows.Sum(r => r.Quantity)
        };
    }
}
