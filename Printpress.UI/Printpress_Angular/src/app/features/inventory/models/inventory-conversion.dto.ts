export type InventoryConversionStatus = 'Open' | 'Completed';

export interface InventoryConversionCreateDto {
  inventoryItemId: string;
  quantity: number;
  notes: string;
  occurredAt: string;
}

export interface InventoryConversionListRowDto {
  id: string;
  occurredAt: string;
  itemId: string;
  itemName: string;
  categoryName: string;
  quantity: number;
  notes: string;
  status: InventoryConversionStatus;
  completedAt: string | null;
  completedBy: string | null;
  createdAt: string;
  createdBy: string;
  isVoided: boolean;
  voidReason: string | null;
  voidedAt: string | null;
  voidedBy: string | null;
}

export interface InventoryConversionListDto {
  rows: InventoryConversionListRowDto[];
  count: number;
  totalQuantity: number;
}
