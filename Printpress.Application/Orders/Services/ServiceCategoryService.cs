using Printpress.Domain;

namespace Printpress.Application;

internal sealed class ServiceCategoryService(IUnitOfWork _unitOfWork) : IServiceCategoryService
{
    public async Task<List<ServiceCategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.ServiceCategoryRepository.AllAsync();

        return categories.Select(c => new ServiceCategoryDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            RequireInventoryItem = c.RequireInventoryItem
        }).ToList();
    }
}
