export interface SparePartTransactionDto {
  id: string;
  inventoryTransactionType: string;
  quantity: number;
  notes: string;
  createdAt: string;
  referenceLabel?: string | null;
  referenceRoute?: string | null;
}
