using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Printpress.Application;

namespace Printpress.API;

[Route("api/[controller]")]
[AllowAnonymous]
public class ServiceCategoryController(IServiceCategoryService _serviceCategoryService) : AppBaseController
{
    [HttpGet]
    [Route("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _serviceCategoryService.GetAllAsync();
        return Ok(result);
    }
}
