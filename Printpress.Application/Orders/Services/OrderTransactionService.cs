using Printpress.Domain;

namespace Printpress.Application;

internal sealed class OrderTransactionService(
    IUnitOfWork _unitOfWork, OrderTransactionMapper _orderTransactionMapper, 
    ILocalizationService _loc, CachAccountDomainService _cachAccountDomainService) : IOrderTransactionService
{
    public async Task<OrderTransactionDto> AddAsync(OrderTransactionAddDto payload, string userId)
    {
        ValidateTransactionPayload(payload);

        var order = _unitOfWork.OrderRepository.Find(payload.OrderId);

        ValidatePayloadAmountComparedToOrder(order, payload);

        
        var client = await _unitOfWork.OrderTransactionRepository.AddAsync(_orderTransactionMapper.MapFromDestinationToSource(payload));

        var isPayment = EnumHelper.MapStringToEnum<OrderTransactionType>(payload.TransactionType) == OrderTransactionType.Payment;

        var transactionAmount = isPayment ? payload.Amount : (-1 * payload.Amount);

        order.TotalPaid = order.TotalPaid.GetValueOrDefault() + transactionAmount;

        _unitOfWork.OrderRepository.Update(order);

        await AddCachAccountTransaction(payload, isPayment);


        await _unitOfWork.SaveChangesAsync(userId);

        return _orderTransactionMapper.MapFromSourceToDestination(client);
    }

    private async Task AddCachAccountTransaction(OrderTransactionAddDto payload, bool isPayment)
    {

        var cachAccount = await _unitOfWork.CashAccountRepository.FirstOrDefaultAsync(x => x.Type == CashAccountType.Main)
            ?? throw new ValidationExeption(_loc.Get(LocalizationKeys.CashAccounts.NotFound));

        var transactionType = isPayment ? CashTransactionType.In : CashTransactionType.Out;

        _cachAccountDomainService.AddCachAccountTransaction(
            cachAccount,
            transactionType,
            CashTransactionCategory.Sales,
            CashTransactionReferenceType.Order,
            payload.OrderId,
            payload.Amount,
            payload.Note,
            DateTime.UtcNow
        );
    }

    private void ValidateTransactionPayload(OrderTransactionAddDto payload)
    {
        if (!EnumHelper.IsValidEnumValue(typeof(OrderTransactionType), payload.TransactionType))
        {
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.InvalidTransactionType));
        }
        if (payload.Amount <= 0)
        {
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.AmountMustBePositive));
        }
    }

    private void ValidatePayloadAmountComparedToOrder(Order order, OrderTransactionAddDto payload)
    {
        if (EnumHelper.MapStringToEnum<OrderTransactionType>(payload.TransactionType) == OrderTransactionType.Payment &&
            payload.Amount > (order.TotalPrice - order.TotalPaid))
        {
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.PaymentExceedsRemaining));
        }
        if (EnumHelper.MapStringToEnum<OrderTransactionType>(payload.TransactionType) == OrderTransactionType.Refund &&
            payload.Amount > order.TotalPaid)
        {
            throw new ValidationExeption(_loc.Get(LocalizationKeys.Orders.RefundExceedsPaid));
        }
    }

    public async Task<PagedList<OrderTransactionDto>> GetByPage(Guid orderId, int pageNumber, int pageSize)
    {

        PagedList<OrderTransaction> pagedList = await _unitOfWork.OrderTransactionRepository.FilterAsync(
            new Paging(pageNumber, pageSize),
            (transaction) => transaction.OrderId == orderId,
            new Sorting(nameof(OrderTransaction.Id), SortingDirection.DESC)
            );

        // check if no data returned then return no data founds

        var result = _orderTransactionMapper.MapFromSourceToDestination(pagedList);

        return result;
    }
}
