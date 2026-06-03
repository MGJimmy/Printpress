using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.Infrastructure
{
    internal class WorkerSalaryTransactionRepository : GenericRepository<WorkerSalaryTransaction>, IWorkerSalaryTransactionRepository
    {
        public WorkerSalaryTransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<WorkerSalaryTransaction> GetThisMonthSalaryTransactions(Guid workerId)
        {
            var now = DateTime.UtcNow;
            return Context.WorkerSalaryTransaction
                .Where(t => t.WorkerId == workerId
                            && t.TransactionDate.Year == now.Year
                            && t.TransactionDate.Month == now.Month)
                .ToList();
        }
    }
}
