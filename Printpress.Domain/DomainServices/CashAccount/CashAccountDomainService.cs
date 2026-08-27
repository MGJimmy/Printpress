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

        public CashTransaction Void(
            CashAccount cashAccount,
            CashTransaction original,
            string reversingDescription,
            DateTime transactionDate)
        {
            EnsureCanCreateReversal(original);

            original.MarkAsVoided();

            var oppositeType = original.Type == CashTransactionType.In
                ? CashTransactionType.Out
                : CashTransactionType.In;

            var reversing = AddCashAccountTransaction(
                cashAccount,
                oppositeType,
                original.Category,
                original.ReferenceType,
                original.ReferenceId,
                original.Amount,
                reversingDescription,
                transactionDate);

            reversing.SetReversesTransactionId(original.Id);
            return reversing;
        }

        public void Transfer(
            CashAccount source,
            CashAccount destination,
            decimal amount,
            string description,
            DateTime transactionDate,
            Guid transferId)
        {
            if (source.Id == destination.Id)
                throw new BusinessExceptions(LocalizationKeys.CashAccounts.CannotTransferSameAccount);

            AddCashAccountTransaction(
                source,
                CashTransactionType.Out,
                CashTransactionCategory.Transfer,
                CashTransactionReferenceType.Transfer,
                transferId,
                amount,
                description,
                transactionDate);

            AddCashAccountTransaction(
                destination,
                CashTransactionType.In,
                CashTransactionCategory.Transfer,
                CashTransactionReferenceType.Transfer,
                transferId,
                amount,
                description,
                transactionDate);
        }

        public static bool CanVoidFromVault(CashTransaction transaction)
        {
            if (!CanCreateReversal(transaction))
                return false;

            if (transaction.ReferenceType == CashTransactionReferenceType.WorkerSalaryTransaction)
                return false;

            if (transaction.Category is CashTransactionCategory.Sales or CashTransactionCategory.SalesReturn)
                return false;

            if (transaction.ReferenceType is CashTransactionReferenceType.PurchaseInventoryInvoice
                or CashTransactionReferenceType.PurchaseSparePartInvoice
                or CashTransactionReferenceType.SellingSparePartInvoice)
                return false;

            return true;
        }

        public static bool CanCreateReversal(CashTransaction transaction)
        {
            return transaction is not null
                && !transaction.IsVoided
                && transaction.ReversesTransactionId is null;
        }

        private static void EnsureCanCreateReversal(CashTransaction original)
        {
            if (original.IsVoided)
                throw new BusinessExceptions(LocalizationKeys.CashAccounts.AlreadyVoided);

            if (original.ReversesTransactionId is not null)
                throw new BusinessExceptions(LocalizationKeys.CashAccounts.CannotVoidReversing);
        }
    }
}
