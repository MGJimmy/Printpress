using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public enum CashTransactionCategory
    {
        Sales = 1,
        Purchases = 2,
        Expenses = 3,
        CapitalInjection = 4,
        Maintenance = 5,
        ExternalServices = 6,
        Other = 99
    }
}
