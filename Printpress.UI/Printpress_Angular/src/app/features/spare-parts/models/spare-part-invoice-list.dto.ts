export interface SparePartInvoiceLineDto {
  id: string;
  itemId: string;
  itemName: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface InvoicePaymentDto {
  id: string;
  amount: number;
  transactionDate: string;
  description: string;
  isVoided: boolean;
}

export interface SparePartPurchaseInvoiceListItemDto {
  id: string;
  invoiceNumber: string;
  invoiceDate: string;
  supplierName: string;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  isGoodsReceived: boolean;
  attachmentFilePath: string;
  createdAt: string;
  isVoided: boolean;
  voidReason?: string | null;
  voidedAt?: string | null;
  voidedBy?: string | null;
  voidedByName?: string | null;
  payments: InvoicePaymentDto[];
  lines: SparePartInvoiceLineDto[];
}

export interface SparePartPurchaseInvoiceListDto {
  invoices: SparePartPurchaseInvoiceListItemDto[];
  invoiceCount: number;
  lineCount: number;
  totalQuantity: number;
  totalAmount: number;
}

export interface SparePartSellingInvoiceListItemDto {
  id: string;
  invoiceNumber: number;
  invoiceDate: string;
  clientName: string;
  totalAmount: number;
  createdAt: string;
  isVoided: boolean;
  voidReason?: string | null;
  voidedAt?: string | null;
  voidedBy?: string | null;
  voidedByName?: string | null;
  lines: SparePartInvoiceLineDto[];
}

export interface SparePartSellingInvoiceListDto {
  invoices: SparePartSellingInvoiceListItemDto[];
  invoiceCount: number;
  lineCount: number;
  totalQuantity: number;
  totalAmount: number;
}
