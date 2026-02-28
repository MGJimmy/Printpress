namespace Printpress.Application
{
    public interface IAccountService
    {
        Task<string> Login(string username, string password);
    }
}
