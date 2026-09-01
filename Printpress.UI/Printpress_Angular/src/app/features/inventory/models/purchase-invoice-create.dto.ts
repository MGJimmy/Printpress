export interface PurchaseInvoiceLineCreateDto {
  inventoryItemId: string;
  quantity: number;
  unitPrice: number;
}

export interface PurchaseInvoiceCreateDto {
  invoiceNumber: string;
  invoiceDate: string;
  supplierName: string;
  attachmentFilePath: string;
  paidNow: number;
  receiveNow: boolean;
  lines: PurchaseInvoiceLineCreateDto[];
}
