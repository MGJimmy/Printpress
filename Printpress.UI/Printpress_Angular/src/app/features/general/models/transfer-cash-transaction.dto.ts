export interface TransferCashTransactionDto {
  fromCashAccountId: string;
  toCashAccountId: string;
  amount: number;
  transactionDate: string;
  description: string;
}
