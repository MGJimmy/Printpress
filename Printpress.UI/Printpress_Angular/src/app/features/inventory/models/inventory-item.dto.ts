export interface InventoryItemDto {
  id: number;
  name: string;
  inventoryItemCategory: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  expectedPurchaseLossPercent: number;
  expectedProductionWastePercent: number;
}
