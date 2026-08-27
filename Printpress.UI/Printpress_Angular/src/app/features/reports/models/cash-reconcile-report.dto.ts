export interface CashReconcileReportDto {
  dateFrom: string | null;
  dateTo: string | null;
  accountCount: number;
  mismatchCount: number;
  totalStoredBalance: number;
  totalComputedBalance: number;
  totalDifference: number;
  accounts: CashReconcileAccountDto[];
}

export interface CashReconcileAccountDto {
  cashAccountId: string;
  cashAccountName: string;
  accountType: string;
  storedBalance: number;
  computedBalance: number;
  difference: number;
  isMatched: boolean;
  openingBalance: number;
  periodIn: number;
  periodOut: number;
  periodClosing: number;
  periodIdentityOk: boolean;
}
