import { IObjectState } from '../../../../core/models/i-object-state';

export interface OrderSellingItemUpsertDto extends IObjectState {
    id: string;
    name: string;
    inventoryItemId?: string;
    isInventoryItem: boolean;
    quantity: number;
    price: number;
}
