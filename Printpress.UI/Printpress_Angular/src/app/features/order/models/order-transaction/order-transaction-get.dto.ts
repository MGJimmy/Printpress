export interface OrderTransactionGetDto {
    id: number;
    orderId: number;
    transactionType: string;
    amount: number;
    createdOn: Date;
}