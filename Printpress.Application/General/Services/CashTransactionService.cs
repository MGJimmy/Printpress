using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Printpress.Domain;

namespace Printpress.Application;

internal sealed class CashTransactionService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IValidator<AddCashTransactionDto> _validator,
    IGuidGenerator _guidGenerator) : ICashTransactionService
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

        var result = await _unitOfWork.CashTransactionRepository.FilterAsync(
            paging,
            t => t.CashAccountId == cashAccountId
                 && (dateFrom == null || t.TransactionDate >= dateFrom)
                 && (dateTo == null || t.TransactionDate <= dateTo)
                 && (typeEnum == null || t.Type == typeEnum)
                 && (categoryEnum == null || t.Category == categoryEnum));

        return new PagedList<CashTransactionDto>
        {
            Items = _mapper.Map<List<CashTransactionDto>>(result.Items),
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

        var account = await _unitOfWork.CashAccountRepository.FindAsync(payload.CashAccountId);
        if (account is null)
            throw new ValidationExeption(ResponseMessage.CreateIdNotExistMessage(payload.CashAccountId));

        var typeEnum = EnumHelper.MapStringToEnum<CashTransactionType>(payload.Type);
        var categoryEnum = EnumHelper.MapStringToEnum<CashTransactionCategory>(payload.Category);
        CashTransactionReferenceType? referenceTypeEnum = null;
        if (!string.IsNullOrEmpty(payload.ReferenceType))
            referenceTypeEnum = EnumHelper.MapStringToEnum<CashTransactionReferenceType>(payload.ReferenceType);

        var transaction = new CashTransaction
        {
            Id = _guidGenerator.NewGuid(),
            CashAccountId = payload.CashAccountId,
            Type = typeEnum,
            Category = categoryEnum,
            ReferenceType = referenceTypeEnum,
            ReferenceId = payload.ReferenceId,
            Amount = payload.Amount,
            Description = payload.Description,
            TransactionDate = payload.TransactionDate
        };

        await _unitOfWork.CashTransactionRepository.AddAsync(transaction);

        if (typeEnum == CashTransactionType.In)
            account.Balance += payload.Amount;
        else
            account.Balance -= payload.Amount;

        _unitOfWork.CashAccountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(userId);

        return _mapper.Map<CashTransactionDto>(transaction);
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
}
