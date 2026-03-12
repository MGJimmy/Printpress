export interface PayrollPeriodDto {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  isClosed: boolean;
  closedAt?: string;
}

export interface PayrollPeriodCreateDto {
  name: string;
  startDate: string;
  endDate: string;
}

export interface WorkerSalaryTransactionDto {
  id: string;
  workerName: string;
  transactionType: number;
  amount: number;
  transactionDate: string;
  note: string;
}

export interface PayrollPeriodDetailsDto extends PayrollPeriodDto {
  transactions: WorkerSalaryTransactionDto[];
}

export const SalaryTransactionTypeLabels: Record<number, string> = {
  1: 'سلفة',
  2: 'دفعة يومية',
  3: 'راتب شهري',
  4: 'مكافأة',
  5: 'خصم / غرامة',
  6: 'تسوية'
};
