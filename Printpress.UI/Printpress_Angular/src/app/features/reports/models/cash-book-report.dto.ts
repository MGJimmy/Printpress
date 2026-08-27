export interface CashBookReportDto {
  cashAccountId: string | null;
  cashAccountName: string | null;
  dateFrom: string | null;
  dateTo: string | null;
  openingBalance: number;
  totalIn: number;
  totalOut: number;
  closingBalance: number;
  accountSummaries: CashBookAccountSummaryDto[];
  lines: CashBookLineDto[];
  totalLineCount: number;
  page: number;
  pageSize: number;
}

export interface CashBookAccountSummaryDto {
  cashAccountId: string;
  cashAccountName: string;
  openingBalance: number;
  totalIn: number;
  totalOut: number;
  closingBalance: number;
}

export interface CashBookLineDto {
  id: string;
  transactionDate: string;
  cashAccountId: string;
  cashAccountName: string;
  inAmount: number;
  outAmount: number;
  runningBalance: number;
  category: string | number;
  description: string;
  status: string;
  createdBy: string;
}
