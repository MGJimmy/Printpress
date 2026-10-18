export interface PurchaseInvoiceLineCreateDto {
  inventoryItemId: string;
  quantity: number;
  unitPrice: number;
}

export interface PurchaseInvoiceCreateDto {
  invoiceDate: string;
  supplierName: string;
  attachmentFilePath: string;
  paidNow: number;
  receiveNow: boolean;
  notes?: string;
  lines: PurchaseInvoiceLineCreateDto[];
}
