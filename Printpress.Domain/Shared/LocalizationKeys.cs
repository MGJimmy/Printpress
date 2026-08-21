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
    }

    public static class CashAccounts
    {

        public const string NotFound = "cashAccounts.not_found";
        public const string addSalaryTransactionDescription = "cashAccounts.addSalaryTransactionDescription";
    }
}
