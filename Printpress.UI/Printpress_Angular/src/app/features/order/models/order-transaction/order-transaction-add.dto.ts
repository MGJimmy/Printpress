export interface OrderTransactionAddDto {
    orderId: string;
    transactionType: string;
    amount: number;
    note?: string;
}