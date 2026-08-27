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

export interface SparePartPurchaseInvoiceListItemDto {
  id: string;
  invoiceNumber: string;
  invoiceDate: string;
  supplierName: string;
  totalAmount: number;
  attachmentFilePath: string;
  createdAt: string;
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
  lines: SparePartInvoiceLineDto[];
}

export interface SparePartSellingInvoiceListDto {
  invoices: SparePartSellingInvoiceListItemDto[];
  invoiceCount: number;
  lineCount: number;
  totalQuantity: number;
  totalAmount: number;
}
