using System;

namespace Printpress.Domain
{
    public class InventoryTransaction : Entity
    {
        public Guid InventoryItemId { get; private set; }
        public InventoryTransactionType InventoryTransactionType { get; private set; }
        public int Quantity { get; private set; }
        public InventoryTransactionReferenceType ReferenceType { get; private set; }

        // Id of the table that transaction related to (Order - PurchaseInvoiceLine - ...)
        public Guid ReferenceId { get; private set; }

        public string Notes { get; private set; }

        public virtual InventoryItem InventoryItem { get; private set; }

        public Guid? WorkerId { get; set; }
        public virtual Worker? Worker { get; set; }

        private InventoryTransaction()
        {
            
        }

        public InventoryTransaction(
        Guid inventoryItemId,
        InventoryTransactionType type,
        int quantity,
        InventoryTransactionReferenceType referenceType,
        Guid referenceId,
        string notes)
        {
            InventoryItemId = inventoryItemId;
            InventoryTransactionType = type;
            Quantity = quantity;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            Notes = notes;
        }

    }
}
