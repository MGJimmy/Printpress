export interface LenderDto {
  id: string;
  name: string;
  phone?: string;
  notes?: string;
}

export interface LenderUpsertDto {
  name: string;
  phone?: string;
  notes?: string;
}

export interface LoanPaymentDto {
  id: string;
  amount: number;
  transactionDate: string;
  description: string;
  isVoided: boolean;
}

export interface LoanListDto {
  id: string;
  loanNumber: number;
  lenderId: string;
  lenderName: string;
  principal: number;
  paidAmount: number;
  remaining: number;
  occurredAt: string;
  isVoided: boolean;
  isClosed: boolean;
}

export interface LoanDto extends LoanListDto {
  notes?: string;
  cashAccountId: string;
  cashAccountName?: string;
  voidReason?: string;
  voidedAt?: string;
  voidedByName?: string;
  payments: LoanPaymentDto[];
}

export interface LoanCreateDto {
  lenderId: string;
  principal: number;
  occurredAt: string;
  cashAccountId: string;
  notes?: string;
}
