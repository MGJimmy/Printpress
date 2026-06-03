using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class WorkerTransactionSummary
    {
        public decimal RemainingAdvances { get; set; }
        public decimal TotalPaidThisMonth { get; set; }
        public decimal TotalBounsThisMonth { get; set; }
        public decimal TotalPenaltyThisMonth { get; set; }
        public decimal? RemainingThisMonth { get; set; }
    }
}
