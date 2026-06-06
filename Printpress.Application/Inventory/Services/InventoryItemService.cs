using AutoMapper;
using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class InventoryItemService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<InventoryItemAddDto> _addValidator,
    IValidator<InventoryItemUpdateDto> _updateValidator,
    IGuidGenerator _guidGenerator) : IInventoryItemService
{
    public async Task<InventoryItemDto> AddAsync(InventoryItemAddDto payload, string userId)
    {
        await ValidateAsync(_addValidator, payload);

        var entity = _mapper.Map<InventoryItem>(payload);
        entity.Id = _guidGenerator.NewGuid();
        var saved = await _unitOfWork.InventoryItemRepository.AddAsync(entity);

        await _unitOfWork.SaveChangesAsync(userId);

        return _mapper.Map<InventoryItemDto>(saved);
    }

    public async Task<InventoryItemDto> UpdateAsync(Guid id, InventoryItemUpdateDto payload, string userId)
    {
        await ValidateAsync(_updateValidator, payload);
        EnsureItemExists(id);

        var entity = _mapper.Map<InventoryItem>(payload);
        entity.Id = id;

        var updated = _unitOfWork.InventoryItemRepository.Update(entity);

        await _unitOfWork.SaveChangesAsync(userId);

        return _mapper.Map<InventoryItemDto>(updated);
    }

    public async Task<InventoryItemDto> GetByIdAsync(Guid id)
    {
        var item = await _unitOfWork.InventoryItemRepository.FindByIdWithStockQuantity(id);

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        return _mapper.Map<InventoryItemDto>(item);
    }

    public async Task<PagedList<InventoryItemDto>> GetAllAsync(Paging paging)
    {
        return await _unitOfWork.InventoryItemRepository.GetAllWithStockQuantity(paging);
    }

    public async Task<List<InventoryItemDto>> GetByCategoryAsync(int categoryId)
    {
        return await _unitOfWork.InventoryItemRepository.GetByCategoryIdAsync(categoryId);
    }

    public async Task DeactivateAsync(Guid id, string userId)
    {
        var item = await _unitOfWork.InventoryItemRepository.FindAsync(id);
        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        item.IsActive = false;
        _unitOfWork.InventoryItemRepository.Update(item);

        var allServices = await _unitOfWork.ServiceRepository.AllAsync();
        foreach (var service in allServices.Where(s => s.InventoryItemId == id))
        {
            service.IsActive = false;
            _unitOfWork.ServiceRepository.Update(service);
        }

        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task DeleteAsync(Guid id, string userId)
    {
        var item = await _unitOfWork.InventoryItemRepository.FindAsync(id);

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        if (_unitOfWork.InventoryTransactionRepository.Any(x => x.InventoryItemId == id))
            throw new ValidationExeption("لا يمكن حذف عنصر المخزون لأن لديه حركات مرتبطة به");

        _unitOfWork.InventoryItemRepository.Remove(item);
        await _unitOfWork.SaveChangesAsync(userId);
    }


    #region Basic Info

    public Task<List<EnumBasicInfoDto>> GetCategoriesLinkToServiceAsync()
    => _unitOfWork.InventoryItemRepository.GetServiceCategoriesAsync();

    public Task<List<EnumBasicInfoDto>> GetCategoriesAllAsync()
        => _unitOfWork.InventoryItemRepository.GetInventoryCategoriesAllAsync();

    public Task<List<EntityBasicInfoDto>> GetItemsByCategoryAsync(int categoryId)
        => _unitOfWork.InventoryItemRepository.GetInventoryItemsForReportAsync(categoryId);

    #endregion


    private static async Task ValidateAsync<T>(IValidator<T> validator, T payload)
    {
        var result = await validator.ValidateAsync(payload);
        if (!result.IsValid)
            throw new ValidationExeption(result.Errors.First().ErrorMessage);
    }

    private void EnsureItemExists(Guid id)
    {
        if (!_unitOfWork.InventoryItemRepository.Any(x => x.Id == id))
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
    }
}
