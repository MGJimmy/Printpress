using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain;

namespace Printpress.Domain
{
    public class CachAccountDomainService
    {
        public void AddCachAccountTransaction(
            CashAccount cashAccount, 
            CashTransactionType cashTransactionType,
            CashTransactionCategory cashTransactionCategory,
            CashTransactionReferenceType? referenceType,
            Guid? referenceId,
            decimal amount,
            string description,
            DateTime transactionDate)
        {
            var transaction = new CashTransaction(
                cashAccount.Id,
                cashTransactionType,
                cashTransactionCategory,
                referenceType,
                referenceId,
                amount,
                description,
                transactionDate);

            cashAccount.AddTransaction(transaction);
        }



    }
}
