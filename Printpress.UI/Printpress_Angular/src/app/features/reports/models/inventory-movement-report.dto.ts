export interface InventoryMovementLineDto {
  id: string;
  movementDate: string;
  type: string;
  inQuantity: number;
  outQuantity: number;
  runningBalance: number;
  referenceType: string;
  workerName: string | null;
  notes: string | null;
}

export interface InventoryMovementReportDto {
  itemId: string;
  itemName: string;
  categoryName: string;
  openingBalance: number;
  totalIn: number;
  totalOut: number;
  closingBalance: number;
  lines: InventoryMovementLineDto[];
}
