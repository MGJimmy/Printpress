namespace Printpress.Application;

public interface ILenderService
{
    Task<List<LenderDto>> GetAllAsync();
    Task<LenderDto> GetByIdAsync(Guid id);
    Task<LenderDto> AddAsync(LenderUpsertDto payload, string userId);
    Task<LenderDto> UpdateAsync(Guid id, LenderUpsertDto payload, string userId);
    Task DeleteAsync(Guid id, string userId);
}

public interface ILoanService
{
    Task<PagedList<LoanListDto>> GetAllAsync(
        Guid? lenderId,
        bool? hasRemaining,
        bool? isVoided,
        DateTime? dateFrom,
        DateTime? dateToExclusive,
        int pageNumber,
        int pageSize);

    Task<LoanDto> GetByIdAsync(Guid id);
    Task<LoanDto> AddAsync(LoanCreateDto payload, string userId);
    Task PayAsync(Guid id, LoanPayDto payload, string userId);
    Task VoidAsync(Guid id, string reason, string userId);
    Task VoidPaymentAsync(Guid id, LoanVoidPaymentDto payload, string userId);
}
