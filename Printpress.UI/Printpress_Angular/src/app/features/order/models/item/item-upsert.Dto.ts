import { IObjectState } from "../../../../core/models/i-object-state";


export interface ItemUpsertDto extends IObjectState {
    id: number;
    name: string;
    quantity: number;
    price: number;
}
