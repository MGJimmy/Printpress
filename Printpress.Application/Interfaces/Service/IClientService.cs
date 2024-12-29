namespace Printpress.Application;

public interface IClientService
{
    Task<ClietntDto> GetClientById(int id);
}