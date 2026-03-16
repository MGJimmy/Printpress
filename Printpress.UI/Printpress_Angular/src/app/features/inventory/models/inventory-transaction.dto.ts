export interface InventoryTransactionDto {
  id: string;
  inventoryTransactionType: string;
  quantity: number;
  referenceType: string;
  referenceId: string;
  notes: string;
  createdAt: string;
  workerId?: string;
  workerName?: string;
}
