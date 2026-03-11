export interface ServiceCategoryDto {
    id: string;
    code: string;
    name: string;
    requireInventoryItem: boolean;
    inventoryItemCategoryId?: number;
    inventoryItemCategoryName?: string;
}
