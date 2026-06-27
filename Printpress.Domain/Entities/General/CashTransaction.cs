using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class CashTransaction : Entity
    {
        public Guid CashAccountId { get; private set; }
        public CashTransactionType Type { get; private set; }
        public CashTransactionCategory Category { get; private set; }
        public CashTransactionReferenceType? ReferenceType { get; private set; }
        public Guid? ReferenceId { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime TransactionDate { get; private set; }

        public virtual CashAccount CashAccount { get; private set; }

        // For EF Core only
        private CashTransaction()
        {
        }

        public CashTransaction(
            Guid cashAccountId, 
            CashTransactionType cashTransactionType, 
            CashTransactionCategory cashTransactionCategory,
            CashTransactionReferenceType? referenceType,
            Guid? referenceId,
            decimal amount,
            string description,
            DateTime transactionDate
            )
        {
            Id = Guid.NewGuid();
            CashAccountId = cashAccountId;
            Type = cashTransactionType;
            Category = cashTransactionCategory;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            Amount = amount;
            Description = description;
            TransactionDate = transactionDate;
        }
    }
}
