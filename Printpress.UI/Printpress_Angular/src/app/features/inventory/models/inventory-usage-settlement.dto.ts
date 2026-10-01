export type InventoryUsageSettlementType = 'ExtraWaste' | 'Theft' | 'Other';

export interface InventoryUsageSettlementCreateDto {
  inventoryItemId: string;
  quantity: number;
  settlementType: InventoryUsageSettlementType;
  notes: string;
  occurredAt: string;
}

export interface InventoryUsageSettlementListRowDto {
  id: string;
  occurredAt: string;
  itemId: string;
  itemName: string;
  categoryName: string;
  quantity: number;
  settlementType: InventoryUsageSettlementType;
  notes: string;
  createdAt: string;
  createdBy: string;
  isVoided: boolean;
  voidReason: string | null;
  voidedAt: string | null;
  voidedBy: string | null;
}

export interface InventoryUsageSettlementListDto {
  rows: InventoryUsageSettlementListRowDto[];
  count: number;
  totalQuantity: number;
}
