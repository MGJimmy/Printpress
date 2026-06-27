using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class CashAccount : Entity
    {

        private List<CashTransaction> _transactions = new();

        public string Name { get; set; }
        public decimal Balance { get; set; }
        public CashAccountType Type { get; set; }

        public IReadOnlyCollection<CashTransaction> Transactions => _transactions.AsReadOnly();

        public void AddTransaction(CashTransaction transaction)
        {
            _transactions.Add(transaction);
            UpdateBalance(transaction);
        }

        private void UpdateBalance(CashTransaction transaction)
        {
            if (transaction.Type == CashTransactionType.In)
            {
                Balance += transaction.Amount;
            }
            else if (transaction.Type == CashTransactionType.Out)
            {
                Balance -= transaction.Amount;
            }
        }
    }
}
