
namespace Printpress.Domain
{
    public class Order : Entity , ISoftDelete
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalPaid { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsZeroOrder { get; set; }

        public OrderStatusEnum Status { get; set; }


        public virtual Client Client { get; set; }
        public virtual List<OrderTransaction> Transactions { get; set; }
        public virtual List<OrderGroup> OrderGroups { get; set; }
        public virtual List<OrderService> Services { get; set; }

        public virtual List<OrderSellingItem> SellingItems { get; set; }

        public void EnsureCanExecute()
        {
            if (Status == OrderStatusEnum.Delivered)
                throw new BusinessExceptions(LocalizationKeys.Orders.CannotExecuteDelivered);
        }

        public bool RefreshStatus(IEnumerable<GroupStatusEnum> groupStatuses)
        {
            if (Status is OrderStatusEnum.Completed or OrderStatusEnum.Delivered)
                return false;

            var statuses = groupStatuses as ICollection<GroupStatusEnum> ?? groupStatuses.ToList();
            if (statuses.Count == 0)
                return false;

            if (statuses.All(status => status is GroupStatusEnum.Completed or GroupStatusEnum.Delivered))
            {
                Status = OrderStatusEnum.Completed;
                return true;
            }

            if (Status == OrderStatusEnum.New &&
                statuses.Any(status => status is GroupStatusEnum.InProgress or GroupStatusEnum.Completed or GroupStatusEnum.Delivered))
            {
                Status = OrderStatusEnum.InProgress;
                return true;
            }

            return false;
        }
    }
}
