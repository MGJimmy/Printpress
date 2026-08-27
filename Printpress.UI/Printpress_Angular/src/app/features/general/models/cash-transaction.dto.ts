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
  isVoided: boolean;
  reversesTransactionId: string | null;
  canVoid: boolean;
  status?: string;
}

export interface ExternalOrderDto {
  orderId: string;
  orderName: string;
}
