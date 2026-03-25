export interface AddCashTransactionDto {
  cashAccountId: string;
  type: string;
  category: string;
  amount: number;
  description: string;
  transactionDate: string;
  referenceId?: string;
  referenceType?: string;
}
