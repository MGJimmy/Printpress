export interface ServiceUsageExecuteRowDto {
  id: string;
  occurredAt: string;
  serviceName: string;
  orderName: string;
  workerName: string | null;
  quantity: number;
  paperUnits: number;
  notes: string | null;
  referenceLabel: string;
  referenceRoute: string;
}

export interface ServiceUsageOrderRowDto {
  orderId: string;
  occurredAt: string;
  serviceName: string;
  orderName: string;
  referenceLabel: string;
  referenceRoute: string;
}

export interface InventoryItemUsageRowDto {
  itemId: string;
  categoryId: number;
  itemCategory: string;
  itemName: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  cartonsIn: number;
  unitsIn: number;
  cartonsOut: number;
  unitsOut: number;
  periodNetCartons: number;
  periodNetUnits: number;
  currentStockCartons: number;
  currentStockUnits: number;
  expectedProductionWastePercent: number;
}

export interface ServiceUsageRowDto {
  serviceName: string;
  orderCount: number;
  itemCount: number;
  paperUsed: number;
}

export interface InventoryServicesUsageReportDto {
  inventoryItems: InventoryItemUsageRowDto[];
  totalCartonsIn: number;
  totalUnitsIn: number;
  totalCartonsOut: number;
  totalUnitsOut: number;
  totalPeriodNetCartons: number;
  totalPeriodNetUnits: number;
  totalCurrentStockCartons: number;
  totalCurrentStockUnits: number;
  services: ServiceUsageRowDto[];
  totalOrders: number;
  totalItems: number;
  totalPaperUsed: number;
  executeRows: ServiceUsageExecuteRowDto[];
  orderRows: ServiceUsageOrderRowDto[];
}

export interface ServiceCategoryFilterDto {
  id: string;
  name: string;
}
