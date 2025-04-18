using Printpress.Domain.Entities;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    internal class OrderTransactionMapper : BaseMapper<OrderTransaction, OrderTransactionDto>
    {
        public override OrderTransaction MapFromDestinationToSource(OrderTransactionDto destinationEntity)
        {
            return new OrderTransaction
            {
                OrderId = destinationEntity.OrderId,
                Amount = destinationEntity.Amount,
                TransactionType = EnumHelper.MapStringToEnum<TransactionType>(destinationEntity.TransactionType)
            };
        }

        public override OrderTransactionDto MapFromSourceToDestination(OrderTransaction entity)
        {
            return new OrderTransactionDto
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                Amount = entity.Amount,
                TransactionType = entity.TransactionType.ToString()
            };
        }

        public OrderTransaction MapFromDestinationToSource(OrderTransactionAddDto destinationEntity)
        {
            return new OrderTransaction
            {
                OrderId = destinationEntity.OrderId,
                Amount = destinationEntity.Amount,
                TransactionType = EnumHelper.MapStringToEnum<TransactionType>(destinationEntity.TransactionType)
            };
        }
    }
}
