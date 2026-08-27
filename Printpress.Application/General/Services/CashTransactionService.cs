using AutoMapper;
using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashTransactionService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<AddCashTransactionDto> _validator,
    IValidator<TransferCashTransactionDto> _transferValidator,
    CashAccountDomainService _cashAccountDomainService,
    CashReferenceResolver _referenceResolver,
    ILocalizationService _loc) : ICashTransactionService
{
    public async Task<PagedList<CashTransactionDto>> GetByCashAccountIdAsync(
        Guid cashAccountId,
        Paging paging,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? type,
        string? category)
    {
        CashTransactionType? typeEnum = null;
        if (!string.IsNullOrEmpty(type))
            typeEnum = EnumHelper.MapStringToEnum<CashTransactionType>(type);

        CashTransactionCategory? categoryEnum = null;
        if (!string.IsNullOrEmpty(category))
            categoryEnum = EnumHelper.MapStringToEnum<CashTransactionCategory>(category);

        var sorting = new Sorting(nameof(CashTransaction.CreatedAt), SortingDirection.DESC);

        var result = await _unitOfWork.CashTransactionRepository.FilterAsync(
            paging,
            t => t.CashAccountId == cashAccountId
                 && (dateFrom == null || t.TransactionDate.Date >= dateFrom.Value.Date)
                 && (dateTo == null || t.TransactionDate.Date <= dateTo.Value.Date)
                 && (typeEnum == null || t.Type == typeEnum)
                 && (categoryEnum == null || t.Category == categoryEnum),
            sorting
            );

        var items = result.Items.ToList();
        var links = await _referenceResolver.ForTransactionsAsync(items);
        var dtos = _mapper.Map<List<CashTransactionDto>>(items);
        foreach (var dto in dtos)
        {
            if (links.TryGetValue(dto.Id, out var link))
            {
                dto.ReferenceLabel = link.Label;
                dto.ReferenceRoute = link.Route;
            }
        }

        return new PagedList<CashTransactionDto>
        {
            Items = dtos,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<CashTransactionDto> AddAsync(AddCashTransactionDto payload, string userId)
    {
        var validationResult = await _validator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var account = await _unitOfWork.CashAccountRepository.FindAsync(payload.CashAccountId)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        var typeEnum = EnumHelper.MapStringToEnum<CashTransactionType>(payload.Type);
        var categoryEnum = EnumHelper.MapStringToEnum<CashTransactionCategory>(payload.Category);
        CashTransactionReferenceType? referenceTypeEnum = null;
        if (!string.IsNullOrEmpty(payload.ReferenceType))
            referenceTypeEnum = EnumHelper.MapStringToEnum<CashTransactionReferenceType>(payload.ReferenceType);

        var transaction = _cashAccountDomainService.AddCashAccountTransaction(
            account,
            typeEnum,
            categoryEnum,
            referenceTypeEnum,
            payload.ReferenceId,
            payload.Amount,
            payload.Description,
            payload.TransactionDate);

        _unitOfWork.CashAccountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(userId);

        return _mapper.Map<CashTransactionDto>(transaction);
    }

    public async Task VoidAsync(Guid transactionId, VoidCashTransactionDto payload, string userId)
    {
        var original = await _unitOfWork.CashTransactionRepository.FindAsync(transactionId)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.TransactionNotFound));

        if (original.ReferenceType == CashTransactionReferenceType.Transfer && original.ReferenceId is Guid transferId)
        {
            await VoidTransferPairAsync(transferId, original, payload?.Reason);
        }
        else
        {
            EnsureVoidableFromVault(original);
            await VoidSingleAsync(original, payload?.Reason);
        }

        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task TransferAsync(TransferCashTransactionDto payload, string userId)
    {
        var validationResult = await _transferValidator.ValidateAsync(payload);
        if (!validationResult.IsValid)
            throw new ValidationExeption(validationResult.Errors.First().ErrorMessage);

        var source = await _unitOfWork.CashAccountRepository.FindAsync(payload.FromCashAccountId)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));
        var destination = await _unitOfWork.CashAccountRepository.FindAsync(payload.ToCashAccountId)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        var description = string.IsNullOrWhiteSpace(payload.Description)
            ? _loc.Get(LocalizationKeys.CashAccounts.TransferDescription, source.Name, destination.Name)
            : payload.Description;

        _cashAccountDomainService.Transfer(
            source,
            destination,
            payload.Amount,
            Truncate(description, 500),
            payload.TransactionDate,
            Guid.NewGuid());

        _unitOfWork.CashAccountRepository.Update(source);
        _unitOfWork.CashAccountRepository.Update(destination);

        await _unitOfWork.SaveChangesAsync(userId);
    }

    public async Task<List<ExternalOrderDto>> GetExternalOrdersAsync()
    {
        var groups = _unitOfWork.OrderGroupRepository.Filter(
            g => (g.ExecutionType == GroupExecutionType.External_WithOurMaterials
                  || g.ExecutionType == GroupExecutionType.External_Full)
                 && g.Order.Status != OrderStatusEnum.Delivered
                 && !g.IsDeleted,
            "Order");

        return groups
            .DistinctBy(g => g.OrderId)
            .Select(g => new ExternalOrderDto
            {
                OrderId = g.OrderId,
                OrderName = g.Order.Name
            }).ToList();
    }

    private async Task VoidTransferPairAsync(Guid transferId, CashTransaction clicked, string reason)
    {
        var legs = (await _unitOfWork.CashTransactionRepository.FilterAsync(
            t => t.ReferenceType == CashTransactionReferenceType.Transfer
                 && t.ReferenceId == transferId
                 && t.ReversesTransactionId == null)).ToList();

        if (legs.Count != 2 || legs.Any(l => l.IsVoided))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.TransferPairIncomplete));

        foreach (var leg in legs.OrderBy(l => l.Type == CashTransactionType.Out ? 0 : 1))
        {
            if (!CashAccountDomainService.CanCreateReversal(leg))
                throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.TransferPairIncomplete));

            await VoidSingleAsync(leg, reason);
        }
    }

    private async Task VoidSingleAsync(CashTransaction original, string reason)
    {
        var account = await _unitOfWork.CashAccountRepository.FindAsync(original.CashAccountId)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        var description = BuildVoidDescription(original.Description, reason);

        _cashAccountDomainService.Void(account, original, description, DateTime.UtcNow);

        _unitOfWork.CashTransactionRepository.Update(original);
        _unitOfWork.CashAccountRepository.Update(account);
    }

    private void EnsureVoidableFromVault(CashTransaction original)
    {
        if (original.IsVoided)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.AlreadyVoided));

        if (original.ReversesTransactionId is not null)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.CannotVoidReversing));

        if (original.ReferenceType == CashTransactionReferenceType.WorkerSalaryTransaction)
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.CannotVoidSalaryFromVault));

        if (!CashAccountDomainService.CanVoidFromVault(original))
            throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.CannotVoidOrderOrInvoice));
    }

    private string BuildVoidDescription(string originalDescription, string reason)
    {
        var text = _loc.Get(LocalizationKeys.CashAccounts.VoidDescription, originalDescription ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(reason))
            text = $"{text} ({reason})";
        return Truncate(text, 500);
    }

    private static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= max)
            return value ?? string.Empty;
        return value[..max];
    }
}
