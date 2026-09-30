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
}

export interface InventoryUsageSettlementListDto {
  rows: InventoryUsageSettlementListRowDto[];
  count: number;
  totalQuantity: number;
}
