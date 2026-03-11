using System;

namespace Printpress.Domain
{
    public class SparePartInventoryTransaction : Entity
    {
        public Guid InventoryItemId { get; private set; }
        public SparePartInventoryTransactionType InventoryTransactionType { get; private set; }
        public int Quantity { get; private set; }

        public string Notes { get; private set; }

        public virtual SparePartInventoryItem InventoryItem { get; private set; }

        private SparePartInventoryTransaction()
        {
            
        }

        public SparePartInventoryTransaction(
        Guid inventoryItemId,
        int quantity,
        string notes)
        {
            InventoryItemId = inventoryItemId;
            Quantity = quantity;
            Notes = notes;
        }

    }
}
