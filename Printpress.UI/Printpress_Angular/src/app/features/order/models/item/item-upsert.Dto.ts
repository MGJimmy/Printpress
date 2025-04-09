import { IObjectState } from "../../../../core/models/i-object-state";
import { ItemDetailsGetDto } from "../item-details/item-details-get.dto";


export interface ItemUpsertDto extends IObjectState {
    id: number;
    name: string;
    quantity: number;
    price: number;
    details: ItemDetailsGetDto[];
}
