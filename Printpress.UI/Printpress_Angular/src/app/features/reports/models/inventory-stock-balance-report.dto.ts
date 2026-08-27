export interface InventoryStockBalanceRowDto {
  itemId: string;
  itemName: string;
  categoryId: number;
  categoryName: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  isActive: boolean;
  openingCartons: number;
  openingUnits: number;
  periodInCartons: number;
  periodInUnits: number;
  periodOutCartons: number;
  periodOutUnits: number;
  closingCartons: number;
  closingUnits: number;
}

export interface InventoryStockBalanceReportDto {
  rows: InventoryStockBalanceRowDto[];
  itemCount: number;
  totalOpeningCartons: number;
  totalPeriodInCartons: number;
  totalPeriodOutCartons: number;
  totalClosingCartons: number;
  totalClosingUnits: number;
}
