using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public interface IInventoryTransactionDomainService
    {
        List<InventoryTransaction> CreateInventoryTransaction(List<PurchaseInvoiceLine> purchaseInvoiceLines);
        List<InventoryTransaction> CreatePurchaseVoidTransactions(List<PurchaseInvoiceLine> purchaseInvoiceLines, string invoiceNumber);
    }
}
