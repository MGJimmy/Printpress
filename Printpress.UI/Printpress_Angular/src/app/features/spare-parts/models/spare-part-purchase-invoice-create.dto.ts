export interface SparePartPurchaseInvoiceLineCreateDto {
  sparePartItemId: string;
  quantity: number;
  unitPrice: number;
}

export interface SparePartPurchaseInvoiceCreateDto {
  invoiceNumber: string;
  invoiceDate: string;
  supplierName: string;
  attachmentFilePath: string;
  paidNow: number;
  receiveNow: boolean;
  lines: SparePartPurchaseInvoiceLineCreateDto[];
}
