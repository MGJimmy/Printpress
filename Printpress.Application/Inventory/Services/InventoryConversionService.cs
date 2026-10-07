using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class InventoryConversionService(
    IUnitOfWork _unitOfWork,
    IValidator<InventoryConversionCreateDto> _createValidator,
    IGuidGenerator _guidGenerator,
    ILocalizationService _loc) : IInventoryConversionService
{
    public async Task CreateAsync(InventoryConversionCreateDto payload, string userId)
    {
        if (payload is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Shared.InvalidPayload));

        var validationResult = await _createValidator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var item = await _unitOfWork.InventoryItemRepository.FindAsync(payload.InventoryItemId);
        if (item is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Inventory.ItemNotFound));

        var conversion = new InventoryConversion(
            payload.InventoryItemId,
            payload.Quantity,
            payload.Notes,
            UtcDateTime.AsUtc(payload.OccurredAt));
        conversion.Id = _guidGenerator.NewGuid();

        await _unitOfWork.InventoryConversionRepository.AddAsync(conversion);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task CompleteAsync(Guid id, string userId)
    {
        var conversion = await _unitOfWork.InventoryConversionRepository.FindAsync(id);
        if (conversion is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Inventory.ConversionNotFound));

        conversion.Complete(userId);
        _unitOfWork.InventoryConversionRepository.Update(conversion);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task VoidAsync(Guid id, string reason, string userId)
    {
        reason = InvoiceVoidHelper.RequireReason(reason, _loc);

        var conversion = await _unitOfWork.InventoryConversionRepository.FindAsync(id);
        if (conversion is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Inventory.ConversionNotFound));

        conversion.MarkAsVoided(reason, userId);
        _unitOfWork.InventoryConversionRepository.Update(conversion);
        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task<InventoryConversionListDto> GetAllAsync(
        int? categoryId,
        Guid? itemId,
        InventoryConversionStatus? status,
        bool? isVoided,
        DateTime? dateFrom,
        DateTime? dateToExclusive)
    {
        InvoiceVoidHelper.EnsureDateRange(dateFrom, dateToExclusive, _loc);

        var conversions = (await _unitOfWork.InventoryConversionRepository.FilterAsync(
                c => (dateFrom == null || c.OccurredAt >= dateFrom)
                    && (dateToExclusive == null || c.OccurredAt < dateToExclusive)
                    && (itemId == null || c.InventoryItemId == itemId)
                    && (categoryId == null || c.InventoryItem.InventoryItemCategoryId == categoryId)
                    && (status == null || c.Status == status)
                    && (isVoided == null || c.IsVoided == isVoided),
                nameof(InventoryConversion.InventoryItem),
                $"{nameof(InventoryConversion.InventoryItem)}.{nameof(InventoryItem.InventoryItemCategory_LKP)}"))
            .OrderByDescending(c => c.OccurredAt)
            .ToList();

        var rows = conversions.Select(c => new InventoryConversionListRowDto
        {
            Id = c.Id,
            OccurredAt = c.OccurredAt,
            ItemId = c.InventoryItemId,
            ItemName = c.InventoryItem?.Name ?? "—",
            CategoryName = c.InventoryItem?.InventoryItemCategory_LKP?.Name ?? "—",
            Quantity = c.Quantity,
            Notes = c.Notes,
            Status = c.Status,
            CompletedAt = c.CompletedAt,
            CompletedBy = c.CompletedBy,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            IsVoided = c.IsVoided,
            VoidReason = c.VoidReason,
            VoidedAt = c.VoidedAt,
            VoidedBy = c.VoidedBy
        }).ToList();

        return new InventoryConversionListDto
        {
            Rows = rows,
            Count = rows.Count,
            TotalQuantity = rows.Where(r => !r.IsVoided).Sum(r => r.Quantity)
        };
    }
}
