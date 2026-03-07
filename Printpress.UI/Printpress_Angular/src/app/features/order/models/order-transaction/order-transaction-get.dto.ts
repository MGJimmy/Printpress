export interface OrderTransactionGetDto {
    id: string;
    orderId: string;
    transactionType: string;
    amount: number;
    createdOn: Date;
}