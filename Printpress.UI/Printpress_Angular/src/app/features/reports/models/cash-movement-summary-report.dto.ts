export interface CashMovementSummaryReportDto {
  dateFrom: string | null;
  dateTo: string | null;
  cashAccountId: string | null;
  totalIn: number;
  totalOut: number;
  net: number;
  transactionCount: number;
  byCategory: CashMovementSliceDto[];
  byAccount: CashMovementSliceDto[];
}

export interface CashMovementSliceDto {
  key: string;
  label: string;
  category: string | number | null;
  cashAccountId: string | null;
  totalIn: number;
  totalOut: number;
  net: number;
  transactionCount: number;
}
