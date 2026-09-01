export interface InventoryPurchaseInvoiceLineDto {
  id: string;
  itemId: string;
  itemName: string;
  categoryName: string;
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

export interface InventoryPurchaseInvoiceListItemDto {
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
  lines: InventoryPurchaseInvoiceLineDto[];
}

export interface InventoryPurchaseInvoiceListDto {
  invoices: InventoryPurchaseInvoiceListItemDto[];
  invoiceCount: number;
  lineCount: number;
  totalQuantity: number;
  totalAmount: number;
}

export interface InventoryTransactionListRowDto {
  id: string;
  createdAt: string;
  itemId: string;
  itemName: string;
  categoryName: string;
  inventoryTransactionType: string;
  quantity: number;
  referenceType: string;
  referenceLabel: string;
  referenceRoute: string;
  workerId?: string | null;
  workerName?: string | null;
  notes: string;
}

export interface InventoryTransactionListDto {
  rows: InventoryTransactionListRowDto[];
  movementCount: number;
  itemCount: number;
  totalInQuantity: number;
  totalOutQuantity: number;
}
