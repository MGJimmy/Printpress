using ItemStatus = Printpress.Domain.OrderItemStatus;

namespace Printpress.Domain
{
    public class OrderItem : Entity , ISoftDelete
    {
        public string Name { get; set; }
        public Guid OrderGroupId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public OrderItemStatus OrderItemStatus { get; set; }

        public virtual OrderGroup OrderGroup { get; set; }
        public virtual List<OrderItemDetails> Details { get; set; }

        public int GetRemaining(int alreadyExecuted) => Quantity - alreadyExecuted;

        public bool AcceptExecution(int quantity, int alreadyExecuted)
        {
            if (OrderItemStatus == ItemStatus.Completed)
                throw new BusinessExceptions(LocalizationKeys.Orders.ItemAlreadyCompleted);

            if (quantity <= 0)
                throw new BusinessExceptions(LocalizationKeys.Orders.ExecutionQuantityMustBePositive);

            var remaining = GetRemaining(alreadyExecuted);
            if (quantity > remaining)
                throw new BusinessExceptions(LocalizationKeys.Orders.ExecutionExceedsRemaining, quantity, remaining);

            if (OrderItemStatus != ItemStatus.New)
                return false;

            OrderItemStatus = ItemStatus.InProgress;
            return true;
        }

        public bool TryComplete(
            IReadOnlyCollection<Guid> requiredCategoryIds,
            IReadOnlyDictionary<Guid, int> executedByCategory)
        {
            if (requiredCategoryIds.Count == 0 || OrderItemStatus == ItemStatus.Completed)
                return false;

            var allServicesComplete = requiredCategoryIds.All(categoryId =>
                executedByCategory.TryGetValue(categoryId, out var executed) && executed >= Quantity);

            if (!allServicesComplete)
                return false;

            OrderItemStatus = ItemStatus.Completed;
            return true;
        }
    }
}
