export interface ServiceGetDto {
    id: string;
    name: string;
    price: number;
    serviceCategoryId: string;
    serviceCategoryCode: string;
    serviceCategoryName: string;
    inventoryItemId?: string;
    inventoryItemName?: string;
}