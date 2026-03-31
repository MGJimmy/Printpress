export interface CashTransactionDto {
  id: string;
  cashAccountId: string;
  type: string;
  category: string;
  referenceType: string | null;
  referenceId: string | null;
  amount: number;
  description: string;
  transactionDate: string;
  createdAt: string;
}

export interface ExternalOrderDto {
  orderId: string;
  orderName: string;
}
