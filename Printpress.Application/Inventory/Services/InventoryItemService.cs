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
        var item = await _unitOfWork.InventoryItemRepository.FindAsync(id);

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        return _mapper.Map<InventoryItemDto>(item);
    }

    public async Task<PagedList<InventoryItemDto>> GetAllAsync(Paging paging)
    {
        var result = await _unitOfWork.InventoryItemRepository.AllAsync(paging);
        return new PagedList<InventoryItemDto>
        {
            Items = _mapper.Map<List<InventoryItemDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task DeleteAsync(Guid id, string userId)
    {
        var item = await _unitOfWork.InventoryItemRepository.FindAsync(id);

        if (item is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(id));

        _unitOfWork.InventoryItemRepository.Remove(item);
        await _unitOfWork.SaveChangesAsync(userId);
    }

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
