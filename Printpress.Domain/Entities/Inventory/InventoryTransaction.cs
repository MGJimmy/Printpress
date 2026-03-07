using System;

namespace Printpress.Domain
{
    public class InventoryTransaction : Entity
    {
        public int InventoryItemId { get; private set; }
        public InventoryTransactionType InventoryTransactionType { get; private set; }
        public int Quantity { get; private set; }
        public InventoryTransactionReferenceType ReferenceType { get; private set; }

        // Id of the table that transaction related to (Order - PurchaseInvoiceLine - ...) 
        public int ReferenceId { get; private set; }

        public string Notes { get; private set; }

        public virtual InventoryItem InventoryItem { get; private set; }

        private InventoryTransaction()
        {
            
        }

        public InventoryTransaction(
        int inventoryItemId,
        InventoryTransactionType type,
        int quantity,
        InventoryTransactionReferenceType referenceType,
        int referenceId,
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
