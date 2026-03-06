using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class PurchaseInvoiceLine : Entity
    {
        public int PurchaseInvoiceId { get; set; }
        public int InventoryItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        public virtual PurchaseInvoice PurchaseInvoice { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
    }
}
