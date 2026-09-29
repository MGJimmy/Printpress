
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
            if (Status == OrderStatusEnum.Closed)
                throw new BusinessExceptions(LocalizationKeys.Orders.CannotExecuteClosed);
        }

        public bool RefreshStatus()
        {
            var groups = ActiveGroups();

            if (Status == OrderStatusEnum.Closed)
                return false;

            var next = groups.Count == 0
                ? OrderStatusEnum.Draft
                : groups.Any(g => g.Status is GroupStatusEnum.InProgress
                    or GroupStatusEnum.Completed
                    or GroupStatusEnum.Delivered)
                    ? OrderStatusEnum.InProgress
                    : OrderStatusEnum.New;

            if (Status == next)
                return false;

            Status = next;
            return true;
        }

        public bool CanClose()
        {
            if (Status == OrderStatusEnum.Closed)
                return false;

            return AllGroupsDelivered();
        }

        public void Close()
        {
            if (Status == OrderStatusEnum.Closed)
                throw new BusinessExceptions(LocalizationKeys.Orders.OrderAlreadyClosed);

            if (!AllGroupsDelivered())
                throw new BusinessExceptions(LocalizationKeys.Orders.CannotCloseOrder);

            Status = OrderStatusEnum.Closed;
        }

        private bool AllGroupsDelivered()
        {
            var groups = ActiveGroups();
            if (groups.Count == 0)
                return false;

            return groups.All(g => g.Status == GroupStatusEnum.Delivered);
        }

        private List<OrderGroup> ActiveGroups()
        {
            if (OrderGroups is null)
                throw new InvalidOperationException("Order groups were not loaded.");

            return OrderGroups.Where(g => !g.IsDeleted).ToList();
        }
    }
}
