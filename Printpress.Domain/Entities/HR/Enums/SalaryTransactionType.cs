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
       DailyPayment = 2,
       MonthlySalary = 3,
       Bonus = 4,
       Penalty = 5,
       Adjustment = 6
    }
}
