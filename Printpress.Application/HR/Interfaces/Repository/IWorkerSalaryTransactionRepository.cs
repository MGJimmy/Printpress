using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain;

namespace Printpress.Application
{
    public interface IWorkerSalaryTransactionRepository : IGenericRepository<WorkerSalaryTransaction>
    {
        IEnumerable<WorkerSalaryTransaction> GetThisMonthSalaryTransactions(Guid workerId);
    }
}
