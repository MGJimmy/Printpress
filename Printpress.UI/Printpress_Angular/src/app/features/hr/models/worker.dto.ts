import { SalaryType } from "./salary-type.enum";

export interface WorkerDto {
  id: string;
  name: string;
  phoneNumber: string;
  address: string;
  notes: string;
  salaryType: SalaryType;
  monthlySalary?: number;
  dailySalary?: number;
  isActive: boolean;
}

export interface WorkerCreateDto {
  name: string;
  phoneNumber: string;
  address: string;
  notes: string;
  salaryType: SalaryType;
  monthlySalary?: number;
  dailySalary?: number;
}

export interface WorkerUpdateDto {
  id: string;
  name: string;
  phoneNumber: string;
  address: string;
  notes: string;
  salaryType: SalaryType;
  monthlySalary?: number;
  dailySalary?: number;
}

export interface WorkerSalaryTransactionDto {
  id: string;
  workerName: string;
  transactionType: number;
  amount: number;
  transactionDate: string;
  note: string;
  payrollPeriodName: string;
}

export interface WorkerProductionDto {
  id: string;
  productionDate: string;
  serviceCategoryName: string;
  orderName: string;
  quantity: number;
  notes: string;
}

export interface WorkerSummaryStatsDto {
  remainingAdvances: number;
  totalPaidThisMonth: number;
  remainingThisMonth?: number;
  totalBounsThisMonth: number;
  totalPenaltyThisMonth: number;
}

export interface WorkerDetailsDto extends WorkerDto {
  transactions: WorkerSalaryTransactionDto[];
  productions: WorkerProductionDto[];
  stats: WorkerSummaryStatsDto;
}

export interface WorkerInventoryTransactionDto {
  id: string;
  inventoryItemName: string;
  inventoryItemCategoryName?: string;
  inventoryTransactionType: string;
  quantity: number;
  notes?: string;
  createdAt: string;
}

export interface AddSalaryTransactionDto {
  workerId: string;
  payrollPeriodId: string;
  transactionType: number;
  amount: number;
  transactionDate: string;
  note: string;
}

export const SalaryTypeLabels: Record<number, string> = {
  1: 'شهري',
  2: 'يومي'
};

export const SalaryTransactionTypeLabels: Record<number, string> = {
  1: 'سلفة',
  2: 'راتب',
  3: 'مكافأة',
  4: 'خصم / غرامة',
  5: 'تسوية',
  6: 'رد سلفة راتب'
};
  