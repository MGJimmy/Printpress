namespace Printpress.Domain;

/// <summary>Strongly-typed localization key constants. Keys match the JSON files under Shared/Localization/.</summary>
public static class LocalizationKeys
{
    public static class Shared
    {
        public const string Success            = "shared.success";
        public const string NoDataFound        = "shared.no_data_found";
        public const string InvalidPayload     = "shared.invalid_payload";
        public const string InternalServerError= "shared.internal_server_error";
        public const string ValidationFailure  = "shared.validation_failure";
        public const string NotFoundById       = "shared.not_found_by_id";
        public const string Required           = "shared.required";
        public const string MaxLength          = "shared.max_length";
        public const string MustBePositive     = "shared.must_be_positive";
    }

    public static class Orders
    {
        public const string OrderNotFound          = "orders.order_not_found";
        public const string GroupNotFound          = "orders.group_not_found";
        public const string GroupAlreadyDelivered  = "orders.group_already_delivered";
        public const string ClientNotFound         = "orders.client_not_found";
        public const string ServiceNotFound        = "orders.service_not_found";
        public const string InvalidTransactionType = "orders.invalid_transaction_type";
        public const string AmountMustBePositive   = "orders.amount_must_be_positive";
        public const string PaymentExceedsRemaining= "orders.payment_exceeds_remaining";
        public const string RefundExceedsPaid      = "orders.refund_exceeds_paid";
        public const string ServiceTypeDuplicate   = "orders.service_type_duplicate";
        public const string PrintingMainDuplicate  = "orders.printing_main_duplicate";
        public const string PrintingCoverDuplicate = "orders.printing_cover_duplicate";
        public const string OrderAlreadyDelivered  = "orders.order_already_delivered";
        public const string CannotDeleteHasChildren = "orders.cannot_delete_has_children";
        public const string CannotDeleteCompletedGroup = "orders.cannot_delete_completed_group";
        public const string CannotDeleteGroupWithExecutions = "orders.cannot_delete_group_with_executions";
        public const string CannotEditExecutedItem = "orders.cannot_edit_executed_item";
        public const string CannotDeleteExecutedItem = "orders.cannot_delete_executed_item";
        public const string CannotChangeServicesAfterExecution = "orders.cannot_change_services_after_execution";
        public const string CannotAddItemToClosedGroup = "orders.cannot_add_item_to_closed_group";
        public const string GroupNotCompletedForDelivery = "orders.group_not_completed_for_delivery";
        public const string CannotExecuteDelivered = "orders.cannot_execute_delivered";
        public const string CannotDeleteOrderWithWork = "orders.cannot_delete_order_with_work";
    }

    public static class CashAccounts
    {
        public const string NotFound = "cashAccounts.not_found";
        public const string InsufficientFunds = "cashAccounts.insufficient_funds";
        public const string ConcurrencyConflict = "cashAccounts.concurrency_conflict";
        public const string addSalaryTransactionDescription = "cashAccounts.addSalaryTransactionDescription";
        public const string PurchaseInvoiceDescription = "cashAccounts.purchase_invoice_description";
        public const string SparePurchaseInvoiceDescription = "cashAccounts.spare_purchase_invoice_description";
        public const string SpareSellingInvoiceDescription = "cashAccounts.spare_selling_invoice_description";
        public const string AlreadyVoided = "cashAccounts.already_voided";
        public const string CannotVoidReversing = "cashAccounts.cannot_void_reversing";
        public const string CannotVoidOrderOrInvoice = "cashAccounts.cannot_void_order_or_invoice";
        public const string CannotVoidSalaryFromVault = "cashAccounts.cannot_void_salary_from_vault";
        public const string CannotTransferSameAccount = "cashAccounts.cannot_transfer_same_account";
        public const string TransferPairIncomplete = "cashAccounts.transfer_pair_incomplete";
        public const string VoidDescription = "cashAccounts.void_description";
        public const string TransferDescription = "cashAccounts.transfer_description";
        public const string InvalidType = "cashAccounts.invalid_type";
        public const string InvalidCategory = "cashAccounts.invalid_category";
        public const string InvalidReferenceType = "cashAccounts.invalid_reference_type";
        public const string FieldCashAccount = "cashAccounts.field_cash_account";
        public const string FieldType = "cashAccounts.field_type";
        public const string FieldCategory = "cashAccounts.field_category";
        public const string FieldAmount = "cashAccounts.field_amount";
        public const string FieldDate = "cashAccounts.field_date";
        public const string FieldDescription = "cashAccounts.field_description";
        public const string FieldReferenceId = "cashAccounts.field_reference_id";
        public const string FieldFromAccount = "cashAccounts.field_from_account";
        public const string FieldToAccount = "cashAccounts.field_to_account";
        public const string TransactionNotFound = "cashAccounts.transaction_not_found";
        public const string InvalidDateRange = "cashAccounts.invalid_date_range";
    }
}
