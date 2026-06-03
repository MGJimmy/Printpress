using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class Worker : Entity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public SalaryType SalaryType { get; set; }
        public decimal? MonthlySalary { get; set; }
        public decimal? DailySalary { get; set; }
        public decimal UnpaidAdvanceAmount { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }


        public virtual ICollection<WorkerSalaryTransaction> SalaryTransactions { get; set; } = [];
        public virtual ICollection<ItemServiceExecution> WorkerProductions { get; set; } = [];
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = [];

        public WorkerSalaryTransaction AddTransaction(
            Guid id,
            Guid payrollPeriodId,
            SalaryTransactionType transactionType,
            decimal amount,
            DateTime transactionDate,
            string? note)
        {
            if(transactionType == SalaryTransactionType.SalaryAdvance)
            {
                UnpaidAdvanceAmount += amount;
            }

            if(transactionType == SalaryTransactionType.SalaryAdvancePayment)
            {
                UnpaidAdvanceAmount -= amount;
            }

            var transaction = new WorkerSalaryTransaction
            {
                Id = id,
                WorkerId = Id,
                PayrollPeriodId = payrollPeriodId,
                TransactionType = transactionType,
                Amount = amount,
                TransactionDate = transactionDate,
                Note = note
            };

            SalaryTransactions.Add(transaction);

            return transaction;
        }

    }
}
