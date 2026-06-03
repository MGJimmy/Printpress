using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public enum SalaryTransactionType
    {
       SalaryAdvance = 1,
       Salary = 2,
       Bonus = 3,
       Penalty = 4,
       Adjustment = 5,
       SalaryAdvancePayment = 6
    }
}
