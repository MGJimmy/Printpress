using AutoMapper;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashAccountService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    ILocalizationService _loc) : ICashAccountService
{
    public async Task<List<CashAccountDto>> GetAllAsync()
    {
        var accounts = await _unitOfWork.CashAccountRepository.AllAsync();
        return _mapper.Map<List<CashAccountDto>>(accounts);
    }

    public async Task<CashAccountDto> GetByIdAsync(Guid id)
    {
        var account = await _unitOfWork.CashAccountRepository.FindAsync(id);
        if (account is null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        return _mapper.Map<CashAccountDto>(account);
    }
}
