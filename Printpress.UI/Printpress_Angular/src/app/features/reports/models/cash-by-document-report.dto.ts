export interface CashByDocumentReportDto {
  dateFrom: string | null;
  dateTo: string | null;
  cashAccountId: string | null;
  totalIn: number;
  totalOut: number;
  documents: CashDocumentGroupDto[];
}

export interface CashDocumentGroupDto {
  referenceType: string | number | null;
  referenceId: string | null;
  referenceTypeName: string;
  transactionCount: number;
  totalIn: number;
  totalOut: number;
  net: number;
}
