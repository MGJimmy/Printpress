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

export interface InventoryPurchaseInvoiceListItemDto {
  id: string;
  invoiceNumber: string;
  invoiceDate: string;
  supplierName: string;
  totalAmount: number;
  attachmentFilePath: string;
  createdAt: string;
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
