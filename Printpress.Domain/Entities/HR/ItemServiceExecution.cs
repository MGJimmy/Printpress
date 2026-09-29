namespace Printpress.Domain
{
    public class ItemServiceExecution : Entity
    {
        public Guid WorkerId { get; set; }
        public Guid OrderItemId { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public virtual Worker Worker { get; set; }
        public virtual OrderItem OrderItem { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }

        public static ItemServiceExecution Create(
            Guid id,
            Guid orderItemId,
            Guid serviceCategoryId,
            Guid workerId,
            int quantity,
            DateTime executionDate,
            string notes)
        {
            if (quantity <= 0)
                throw new BusinessExceptions(LocalizationKeys.Orders.ExecutionQuantityMustBePositive);

            if (workerId == Guid.Empty)
                throw new BusinessExceptions(LocalizationKeys.Orders.WorkerRequired);

            if (orderItemId == Guid.Empty || serviceCategoryId == Guid.Empty)
                throw new BusinessExceptions(LocalizationKeys.Shared.InvalidPayload);

            return new ItemServiceExecution
            {
                Id = id,
                OrderItemId = orderItemId,
                ServiceCategoryId = serviceCategoryId,
                WorkerId = workerId,
                Quantity = quantity,
                ExecutionDate = executionDate,
                Notes = notes
            };
        }
    }
}
