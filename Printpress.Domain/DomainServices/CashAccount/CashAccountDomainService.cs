namespace Printpress.Domain
{
    public class CashAccountDomainService
    {
        public CashTransaction AddCashAccountTransaction(
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
            return transaction;
        }

        public void RemoveCashAccountTransaction(CashAccount cashAccount, CashTransaction transaction)
        {
            cashAccount.RemoveTransaction(transaction);
        }
    }
}
