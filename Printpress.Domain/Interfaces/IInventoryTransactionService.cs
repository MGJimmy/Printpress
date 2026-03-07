using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public interface IInventoryTransactionService
    {
        List<InventoryTransaction> CreateInventoryTransaction(List<PurchaseInvoiceLine> purchaseInvoiceLines);
    }
}
