using AutoMapper;
using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class SparePartItemService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<SparePartItemAddDto> _addValidator,
    IValidator<SparePartItemUpdateDto> _updateValidator,
    IGuidGenerator _guidGenerator) : ISparePartItemService
{
    public async Task<PagedList<SparePartItemDto>> GetAllAsync(Paging paging)
    {
        return await _unitOfWork.SparePartItemRepository.GetAllWithStockQuantityAsync(paging);
    }

    public async Task<List<SparePartItemDto>> GetAllForSelectionAsync()
    {
        return await _unitOfWork.SparePartItemRepository.GetAllForSelectionAsync();
    }

    public async Task<SparePartItemDto> GetByIdAsync(Guid id)
    {
        var item = await _unitOfWork.SparePartItemRepository.FindByIdWithStockQuantityAsync(id);
        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));
        return item;
    }

    public async Task<SparePartItemDto> AddAsync(SparePartItemAddDto payload, string userId)
    {
        var result = await _addValidator.ValidateAsync(payload);
        if (!result.IsValid)
            throw new ValidationExeption(result.Errors.First().ErrorMessage);

        var entity = _mapper.Map<SparePartInventoryItem>(payload);
        entity.Id = _guidGenerator.NewGuid();

        await _unitOfWork.SparePartItemRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(userId);

        return await GetByIdAsync(entity.Id);
    }

    public async Task<SparePartItemDto> UpdateAsync(Guid id, SparePartItemUpdateDto payload, string userId)
    {
        var result = await _updateValidator.ValidateAsync(payload);
        if (!result.IsValid)
            throw new ValidationExeption(result.Errors.First().ErrorMessage);

        if (!_unitOfWork.SparePartItemRepository.Any(x => x.Id == id))
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        var entity = _mapper.Map<SparePartInventoryItem>(payload);
        entity.Id = id;

        _unitOfWork.SparePartItemRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(userId);

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id, string userId)
    {
        var item = await _unitOfWork.SparePartItemRepository.FindAsync(id);
        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        if (_unitOfWork.SparePartTransactionRepository.Any(x => x.InventoryItemId == id))
            throw new ValidationExeption("لا يمكن حذف القطعة لوجود حركات مرتبطة بها");

        _unitOfWork.SparePartItemRepository.Remove(item);
        await _unitOfWork.SaveChangesAsync(userId);
    }
}
