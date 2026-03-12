using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class WorkerSalaryTransaction : Entity
    {
        public Guid WorkerId { get; set; }
        public Guid PayrollPeriodId { get; set; }
        public SalaryTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }


        public virtual Worker Worker { get; set; }
        public virtual PayrollPeriod PayrollPeriod { get; set; }
    }
}
