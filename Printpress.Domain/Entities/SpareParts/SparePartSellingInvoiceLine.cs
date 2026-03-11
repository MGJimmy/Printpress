using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class SparePartSellingInvoiceLine : Entity
    {
        public Guid SellingInvoiceId { get; set; }
        public Guid InventoryItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        public virtual SparePartSellingInvoice SellingInvoice { get; set; }
        public virtual SparePartInventoryItem InventoryItem { get; set; }
    }
}
