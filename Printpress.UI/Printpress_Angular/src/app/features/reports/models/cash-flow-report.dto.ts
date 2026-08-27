export interface CashFlowReportDto {
  dateFrom: string | null;
  dateTo: string | null;
  cashAccountId: string | null;
  totalIn: number;
  totalOut: number;
  net: number;
  byDay: CashFlowBucketDto[];
  byMonth: CashFlowBucketDto[];
}

export interface CashFlowBucketDto {
  key: string;
  label: string;
  periodStart: string | null;
  periodEnd: string | null;
  totalIn: number;
  totalOut: number;
  net: number;
  transactionCount: number;
}
