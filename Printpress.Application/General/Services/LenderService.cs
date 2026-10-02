using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class LenderService(
    IUnitOfWork unitOfWork,
    IValidator<LenderUpsertDto> validator,
    IGuidGenerator guidGenerator,
    ILocalizationService loc) : ILenderService
{
    public async Task<List<LenderDto>> GetAllAsync()
    {
        var lenders = await unitOfWork.LenderRepository.AllAsync();
        return lenders.OrderBy(l => l.Name).Select(Map).ToList();
    }

    public async Task<LenderDto> GetByIdAsync(Guid id)
    {
        var lender = await unitOfWork.LenderRepository.FindAsync(id)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.Lenders.NotFound));
        return Map(lender);
    }

    public async Task<LenderDto> AddAsync(LenderUpsertDto payload, string userId)
    {
        await ValidateAsync(payload);

        var lender = new Lender(payload.Name, payload.Phone, payload.Notes);
        lender.Id = guidGenerator.NewGuid();
        await unitOfWork.LenderRepository.AddAsync(lender);
        await unitOfWork.SaveChangesAsync(userId);
        return Map(lender);
    }

    public async Task<LenderDto> UpdateAsync(Guid id, LenderUpsertDto payload, string userId)
    {
        await ValidateAsync(payload);

        var lender = await unitOfWork.LenderRepository.FindAsync(id)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.Lenders.NotFound));

        lender.Update(payload.Name, payload.Phone, payload.Notes);
        unitOfWork.LenderRepository.Update(lender);
        await unitOfWork.SaveChangesAsync(userId);
        return Map(lender);
    }

    public async Task DeleteAsync(Guid id, string userId)
    {
        var lender = await unitOfWork.LenderRepository.FindAsync(id)
            ?? throw new ValidationExeption(loc.Get(LocalizationKeys.Lenders.NotFound));

        if (await unitOfWork.LoanRepository.AnyAsync(l => l.LenderId == id))
            throw new ValidationExeption(loc.Get(LocalizationKeys.Lenders.HasLoans));

        unitOfWork.LenderRepository.Remove(lender);
        await unitOfWork.SaveChangesAsync(userId);
    }

    private async Task ValidateAsync(LenderUpsertDto payload)
    {
        if (payload is null)
            throw new ValidationExeption(loc.Get(LocalizationKeys.Shared.InvalidPayload));

        var result = await validator.ValidateAsync(payload);
        if (!result.IsValid)
            throw new ValidationExeption(result.Errors.First().ErrorMessage);
    }

    private static LenderDto Map(Lender lender) => new()
    {
        Id = lender.Id,
        Name = lender.Name,
        Phone = lender.Phone,
        Notes = lender.Notes
    };
}
