export interface ServiceUpsertDto {
    name: string;
    price: number;
    serviceCategoryId: string;
    inventoryItemId?: string | null;
}