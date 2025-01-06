namespace Printpress.Application;

public interface IClientService
{
    Task<ClientDto> GetClientById(int id);
}