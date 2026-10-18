export interface SparePartSellingInvoiceLineCreateDto {
  sparePartItemId: string;
  quantity: number;
  unitPrice: number;
}

export interface SparePartSellingInvoiceCreateDto {
  clientName: string;
  invoiceDate: string;
  notes?: string;
  lines: SparePartSellingInvoiceLineCreateDto[];
}
