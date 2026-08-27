export interface InventoryStockOutRowDto {
  id: string;
  movementDate: string;
  itemId: string;
  itemName: string;
  categoryName: string;
  quantity: number;
  workerId: string | null;
  workerName: string | null;
  notes: string | null;
}

export interface InventoryStockOutReportDto {
  rows: InventoryStockOutRowDto[];
  movementCount: number;
  totalCartons: number;
  itemCount: number;
  workerCount: number;
}
