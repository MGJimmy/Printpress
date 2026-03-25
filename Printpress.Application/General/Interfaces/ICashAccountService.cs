namespace Printpress.Application;

public interface ICashAccountService
{
    Task<List<CashAccountDto>> GetAllAsync();
    Task<CashAccountDto> GetByIdAsync(Guid id);
}
