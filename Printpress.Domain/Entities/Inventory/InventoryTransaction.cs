using System;

namespace Printpress.Domain
{
    public class InventoryTransaction : Entity
    {
        public int InventoryItemId { get; set; }
        public InventoryTransactionType InventoryTransactionType { get; set; }
        public int Quantity { get; set; }
        public InventoryTransactionReferenceType ReferenceType { get; set; }
        
        // Id of the table that transaction related to (Order - InventoryInvoices - ...) 
        public int ReferenceId { get; set; }

        public string Notes { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }
    }
}
