export interface InventoryItemDto {
  id: string;
  name: string;
  inventoryItemCategory: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  expectedPurchaseLossPercent: number;
  expectedProductionWastePercent: number;
  stockQuantity: number;
  totalInQuantity: number;
  totalOutQuantity: number;
  hasTransactions: boolean;
  isActive: boolean;
}
