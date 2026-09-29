
namespace Printpress.Domain
{
    public class OrderGroup : Entity, ISoftDelete
    {
        public string Name { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryName { get; set; }
        public string ReceiverName { get; set; }
        public string DeliveryNotes { get; set; }
        public GroupStatusEnum Status { get; set; }

        public bool IsDeleted { get; set; }


        public virtual Order Order { get; set; }
        public virtual List<OrderItem> Items { get; set; }
        public virtual List<OrderGroupService> OrderGroupServices { get; set; }

        public void EnsureCanExecute(IEnumerable<Guid> requestedCategoryIds, IReadOnlySet<Guid> groupCategoryIds)
        {
            if (Status == GroupStatusEnum.Delivered)
                throw new BusinessExceptions(LocalizationKeys.Orders.CannotExecuteDelivered);

            if (requestedCategoryIds.Any(id => !groupCategoryIds.Contains(id)))
                throw new BusinessExceptions(LocalizationKeys.Orders.ServiceNotFound);
        }

        public bool RefreshStatus(IEnumerable<OrderItemStatus> itemStatuses)
        {
            if (Status is GroupStatusEnum.Completed or GroupStatusEnum.Delivered)
                return false;

            var statuses = itemStatuses as ICollection<OrderItemStatus> ?? itemStatuses.ToList();
            if (statuses.Count == 0)
                return false;

            if (statuses.All(status => status == OrderItemStatus.Completed))
            {
                Status = GroupStatusEnum.Completed;
                return true;
            }

            if (Status == GroupStatusEnum.New &&
                statuses.Any(status => status is OrderItemStatus.InProgress or OrderItemStatus.Completed))
            {
                Status = GroupStatusEnum.InProgress;
                return true;
            }

            return false;
        }
    }
}
