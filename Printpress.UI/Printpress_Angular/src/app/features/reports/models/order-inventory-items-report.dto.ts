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
  conversionUnits: number;
  expectedWaste: number;
  difference: number;
  settlementUnits: number;
  unexplainedDifference: number;
  currentStockCartons: number;
  currentStockUnits: number;
  periodNetCartons: number;
  periodNetUnits: number;
  outRows: ConsumptionOutRowDto[];
  executeRows: ConsumptionExecuteRowDto[];
  sellingRows: ConsumptionSellingRowDto[];
  conversionRows: ConsumptionConversionRowDto[];
  settlementRows: ConsumptionSettlementRowDto[];
}

export interface ConsumptionSourceLink {
  occurredAt: string;
  notes?: string | null;
  referenceLabel: string;
  referenceRoute: string;
}

export interface ConsumptionOutRowDto extends ConsumptionSourceLink {
  id: string;
  cartons: number;
  units: number;
  workerName?: string | null;
}

export interface ConsumptionExecuteRowDto extends ConsumptionSourceLink {
  id: string;
  orderName: string;
  workerName?: string | null;
  quantity: number;
  numberOfPages: number;
  numberOfPrintingFaces: number;
  isCover: boolean;
  paperUnits: number;
}

export interface ConsumptionSellingRowDto extends ConsumptionSourceLink {
  id: string;
  orderName: string;
  cartons: number;
  units: number;
}

export interface ConsumptionConversionRowDto extends ConsumptionSourceLink {
  id: string;
  quantity: number;
}

export interface ConsumptionSettlementRowDto extends ConsumptionSourceLink {
  id: string;
  quantity: number;
  settlementType: string;
}

export interface InventoryCategoryFilterDto {
  id: number;
  name: string;
}

export interface InventoryItemFilterDto {
  id: string;
  name: string;
}
