using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        public List<InventoryTransaction> CreateInventoryTransaction(List<PurchaseInvoiceLine> purchaseInvoiceLines)
        {
            return purchaseInvoiceLines.Select(x => new InventoryTransaction(
                 x.InventoryItemId,
                 InventoryTransactionType.In,
                 (int)x.Quantity,
                 InventoryTransactionReferenceType.Purchase,
                 x.Id,
                 $"Purchase of {x.Quantity} units at price {x.UnitPrice} per unit"
             )).ToList();
        }
    }
}
