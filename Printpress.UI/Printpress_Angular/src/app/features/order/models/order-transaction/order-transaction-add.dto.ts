export interface OrderTransactionAddDto {
    orderId: number;
    transactionType: string;
    amount: number;
    note?: string;
}