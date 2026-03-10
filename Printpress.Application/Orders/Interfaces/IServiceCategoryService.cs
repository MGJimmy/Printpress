namespace Printpress.Application;

public interface IServiceCategoryService
{
    Task<List<ServiceCategoryDto>> GetAllAsync();
}
