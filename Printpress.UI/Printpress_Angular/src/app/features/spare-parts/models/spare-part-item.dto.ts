export interface SparePartItemDto {
  id: string;
  name: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  stockQuantity: number;
  totalInQuantity: number;
  totalOutQuantity: number;
  hasTransactions: boolean;
}
