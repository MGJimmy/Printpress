import { IObjectState } from '../../../../core/models/i-object-state';

export interface OrderSellingItemGetDto extends IObjectState {
    id: string;
    name: string;
    orderId: string;
    inventoryItemId?: string;
    isInventoryItem: boolean;
    quantity: number;
    price: number;
    inventoryItemName?: string;
}
