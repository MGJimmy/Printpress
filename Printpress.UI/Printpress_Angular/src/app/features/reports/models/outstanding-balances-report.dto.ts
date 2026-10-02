export type OutstandingBalanceDirection = 'Collect' | 'Pay';

export type OutstandingBalanceKind =
  | 'Order'
  | 'WorkerAdvance'
  | 'PurchaseInvoice'
  | 'SparePartPurchaseInvoice'
  | 'Loan'
  | 'MonthlySalary';

export interface OutstandingBalanceRowDto {
  direction: OutstandingBalanceDirection;
  type: OutstandingBalanceKind;
  partyName: string;
  documentLabel: string;
  date: string;
  total: number | null;
  paid: number | null;
  remaining: number;
  route: string;
}

export interface OutstandingBalancesReportDto {
  totalToCollect: number;
  totalToPay: number;
  rows: OutstandingBalanceRowDto[];
}
