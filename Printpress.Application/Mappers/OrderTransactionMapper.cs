using Printpress.Domain.Entities;

namespace Printpress.Application
{
    public class OrderTransactionMapper : BaseMapper<OrderTransaction, OrderTransactionDto>
    {
        public override OrderTransaction MapFromDestinationToSource(OrderTransactionDto destinationEntity)
        {
            return new OrderTransaction
            {
                OrderId = destinationEntity.OrderId,
                Amount = destinationEntity.Amount,
                TransactionType = destinationEntity.TransactionType
            };
        }

        public override OrderTransactionDto MapFromSourceToDestination(OrderTransaction entity)
        {
            return new OrderTransactionDto
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                Amount = entity.Amount,
                TransactionType = entity.TransactionType
            };
        }

        public OrderTransaction MapFromDestinationToSource(OrderTransactionAddDto destinationEntity)
        {
            return new OrderTransaction
            {
                OrderId = destinationEntity.OrderId,
                Amount = destinationEntity.Amount,
                TransactionType = destinationEntity.TransactionType
            };
        }
    }
}
