namespace Printpress.Domain
{
    public class OrderSellingItem : Entity
    {
        public string Name { get; set; }
        public Guid OrderId { get; set; }

        public Guid? InventoryItemId { get; set; }
        public bool IsInventoryItem { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public bool IsDelivered { get; private set; }
        public DateTime? DeliveryDate { get; private set; }
        public string DeliveryName { get; private set; }
        public string ReceiverName { get; private set; }
        public string DeliveryNotes { get; private set; }

        public virtual Order Order { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }

        public void MarkDelivered(DateTime date, string deliveredFrom, string deliveredTo, string notes)
        {
            if (IsDelivered)
                throw new BusinessExceptions(LocalizationKeys.Orders.SellingItemAlreadyDelivered);

            if (date == default)
                throw new BusinessExceptions(LocalizationKeys.Orders.SellingItemDeliveryDateRequired);

            if (string.IsNullOrWhiteSpace(deliveredFrom) || string.IsNullOrWhiteSpace(deliveredTo))
                throw new BusinessExceptions(LocalizationKeys.Orders.SellingItemDeliveryNamesRequired);

            deliveredFrom = deliveredFrom.Trim();
            deliveredTo = deliveredTo.Trim();
            notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

            if (deliveredFrom.Length > 200 || deliveredTo.Length > 200)
                throw new BusinessExceptions(LocalizationKeys.Orders.SellingItemDeliveryMaxLength, 200);

            if (notes is not null && notes.Length > 500)
                throw new BusinessExceptions(LocalizationKeys.Orders.SellingItemDeliveryMaxLength, 500);

            IsDelivered = true;
            DeliveryDate = date;
            DeliveryName = deliveredFrom;
            ReceiverName = deliveredTo;
            DeliveryNotes = notes;
        }

        public void EnsureMutable()
        {
            if (IsDelivered)
                throw new BusinessExceptions(LocalizationKeys.Orders.SellingItemAlreadyDelivered);
        }

        public void RestoreDeliveryFrom(OrderSellingItem persisted)
        {
            IsDelivered = persisted.IsDelivered;
            DeliveryDate = persisted.DeliveryDate;
            DeliveryName = persisted.DeliveryName;
            ReceiverName = persisted.ReceiverName;
            DeliveryNotes = persisted.DeliveryNotes;
        }
    }
}
