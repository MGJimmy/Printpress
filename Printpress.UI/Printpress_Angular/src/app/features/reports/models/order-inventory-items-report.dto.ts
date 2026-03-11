export interface OrderInventoryItemsReportDto {
  itemCategory: string;
  itemName: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  cartonsIn: number;
  unitsIn: number;
  cartonsOut: number;
  unitsOut: number;
  paperUsedUnits: number;
  expectedWaste: number;
  difference: number;
}

export interface InventoryCategoryFilterDto {
  id: number;
  name: string;
}

export interface InventoryItemFilterDto {
  id: string;
  name: string;
}
