namespace Printpress.Domain
{
    public class CashAccount : Entity
    {
        private readonly List<CashTransaction> _transactions = new();

        public string Name { get; set; }
        public decimal Balance { get; set; }
        public CashAccountType Type { get; set; }

        public IReadOnlyCollection<CashTransaction> Transactions => _transactions.AsReadOnly();

        public void AddTransaction(CashTransaction transaction)
        {
            if (transaction.Amount <= 0)
                throw new BusinessExceptions(LocalizationKeys.Orders.AmountMustBePositive);

            if (transaction.Type == CashTransactionType.Out && transaction.Amount > Balance)
                throw new BusinessExceptions(LocalizationKeys.CashAccounts.InsufficientFunds);

            _transactions.Add(transaction);
            ApplyBalance(transaction);
        }

        public void RemoveTransaction(CashTransaction transaction)
        {
            if (transaction.Type == CashTransactionType.In && transaction.Amount > Balance)
                throw new BusinessExceptions(LocalizationKeys.CashAccounts.InsufficientFunds);

            ReverseBalance(transaction);

            var existing = _transactions.FirstOrDefault(t => t.Id == transaction.Id);
            if (existing != null)
                _transactions.Remove(existing);
        }

        private void ApplyBalance(CashTransaction transaction)
        {
            if (transaction.Type == CashTransactionType.In)
                Balance += transaction.Amount;
            else if (transaction.Type == CashTransactionType.Out)
                Balance -= transaction.Amount;
        }

        private void ReverseBalance(CashTransaction transaction)
        {
            if (transaction.Type == CashTransactionType.In)
                Balance -= transaction.Amount;
            else if (transaction.Type == CashTransactionType.Out)
                Balance += transaction.Amount;
        }
    }
}
