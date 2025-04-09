import { IObjectState } from "../../../../core/models/i-object-state";
import { itemDetailsKeyEnum } from "../enums/item-details-key.enum";

export interface ItemDetailsUpsertDto extends IObjectState {
    id: number;
    itemId: number;
    key: itemDetailsKeyEnum;
    value: string;
}