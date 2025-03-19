import { IObjectState } from "../../../../core/models/i-object-state";
import { ItemDetailsGetDto } from "../item-details/item-details-get.dto";

export interface ItemGetDto extends IObjectState {
    id: number;
    name: string;
    quantity: number;
    price: number;
    groupId: number;
    itemDetails: ItemDetailsGetDto[];
}