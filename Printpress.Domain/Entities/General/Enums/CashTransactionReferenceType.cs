using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public enum CashTransactionReferenceType
    {
        Order = 1,
        PurchaseInventoryInvoice = 2,
        PurchaseSparePartInvoice = 3,
        SellingSparePartInvoice = 4,
        WorkerSalaryTransaction = 5,
        Other = 99
    }
}
