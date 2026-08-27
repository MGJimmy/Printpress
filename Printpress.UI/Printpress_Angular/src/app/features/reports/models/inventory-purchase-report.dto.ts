export interface InventoryPurchaseLineRowDto {
  invoiceId: string;
  invoiceNumber: string;
  invoiceDate: string;
  supplierName: string;
  itemId: string;
  itemName: string;
  categoryName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface InventoryPurchaseReportDto {
  lines: InventoryPurchaseLineRowDto[];
  invoiceCount: number;
  lineCount: number;
  totalQuantity: number;
  totalAmount: number;
}
