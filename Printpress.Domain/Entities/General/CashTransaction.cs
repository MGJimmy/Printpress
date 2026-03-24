using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class CashTransaction : Entity
    {
        public Guid CashAccountId { get; set; }
        public CashTransactionType Type { get; set; }
        public CashTransactionCategory Category { get; set; }
        public CashTransactionReferenceType? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public virtual CashAccount CashAccount { get; set; }
    }
}
