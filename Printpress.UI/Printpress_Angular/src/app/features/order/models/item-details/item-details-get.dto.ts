import { IObjectState } from "../../../../core/models/i-object-state";
import { itemDetailsKeyEnum } from "../enums/item-details-key.enum";

export interface ItemDetailsGetDto extends IObjectState {
    id: string;
    itemId: string;
    key: itemDetailsKeyEnum;
    value: string;
}