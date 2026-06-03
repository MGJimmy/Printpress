using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class WorkerTransactionCalculator : IWorkerTransactionCalculator
    {
        public WorkerTransactionSummary Calculate(Worker worker, IEnumerable<WorkerSalaryTransaction> thisMonthTransactions)
        {
            var now = DateTime.UtcNow;
            var firstOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var totalPaidThisMonth = thisMonthTransactions
                .Where(t =>
                 (t.TransactionType == SalaryTransactionType.Salary)
                 || (t.TransactionType == SalaryTransactionType.Bonus)
                 || (t.TransactionType == SalaryTransactionType.Adjustment)
                 )
                .Sum(t => t.Amount);

            var totalBonusThisMonth = thisMonthTransactions
                .Where(t => t.TransactionType == SalaryTransactionType.Bonus)
                .Sum(t => t.Amount);


            var totalPenaltyThisMonth = thisMonthTransactions
                .Where(t => t.TransactionType == SalaryTransactionType.Penalty)
                .Sum(t => t.Amount);

            decimal? remainingThisMonth = null;
            if (worker.SalaryType == SalaryType.Monthly && worker.MonthlySalary.HasValue)
                remainingThisMonth = worker.MonthlySalary.Value - totalPaidThisMonth - totalPenaltyThisMonth;

            return new WorkerTransactionSummary
            {
                RemainingAdvances = worker.UnpaidAdvanceAmount,
                TotalPaidThisMonth = totalPaidThisMonth,
                RemainingThisMonth = remainingThisMonth,
                TotalBounsThisMonth = totalBonusThisMonth,
                TotalPenaltyThisMonth = totalPenaltyThisMonth
            };
        }
    }
}
