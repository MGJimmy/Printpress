export interface CashTreasuryReportDto {
  dateFrom: string | null;
  dateTo: string | null;
  totalStoredBalance: number;
  accounts: CashTreasuryAccountDto[];
  largestIn: CashTreasuryMovementDto[];
  largestOut: CashTreasuryMovementDto[];
  transfers: CashTransferRegisterRowDto[];
}

export interface CashTreasuryAccountDto {
  cashAccountId: string;
  cashAccountName: string;
  accountType: string;
  storedBalance: number;
}

export interface CashTreasuryMovementDto {
  id: string;
  transactionDate: string;
  cashAccountName: string;
  amount: number;
  category: string;
  description: string;
  referenceLabel?: string | null;
  referenceRoute?: string | null;
}

export interface CashTransferRegisterRowDto {
  transferId: string;
  transactionDate: string;
  amount: number;
  fromAccountName: string;
  fromAccountId?: string | null;
  toAccountName: string;
  toAccountId?: string | null;
  description: string;
  isComplete: boolean;
}
