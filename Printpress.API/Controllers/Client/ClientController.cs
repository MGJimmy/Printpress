using Microsoft.AspNetCore.Authorization;

namespace Printpress.API;


[Route("api/[controller]")]
[Authorize]
public class ClientController(IClientService _IClientService) : AppBaseController
{
    [HttpGet]
    [Route("getByPage")]
    public async Task<IActionResult> GetByPage(int pageNumber, int pageSize)
    {
        PagedList<ClientDto> result = await _IClientService.GetByPage(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet]
    [Route("getById")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _IClientService.GetClientById(id);
        return Ok(result);
    }

    [HttpGet]
    [Route("getAll")]
    public async Task<IActionResult> getAll()
    {
        var result = await _IClientService.GetAll();
        return Ok(result);
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add(ClientUpsertDto payload)
    {
        var result = await _IClientService.AddAsync(payload, UserId);
        return Ok(result);
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<IActionResult> Update(int id, ClientUpsertDto payload)
    {
        var result = await _IClientService.UpdateAsync(id, payload, UserId);
        return Ok(result);
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _IClientService.DeleteAsync(id, UserId);
        return Ok();
    }
}
