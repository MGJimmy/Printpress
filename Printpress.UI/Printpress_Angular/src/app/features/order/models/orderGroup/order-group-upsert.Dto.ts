import { IObjectState } from "../../../../core/models/i-object-state";
import { ItemUpsertDto } from "../item/item-upsert.Dto";
import { OrderGroupServiceUpsertDto } from "../orderGroupService/order-group-service-upsert.dto";

export interface OrderGroupUpsertDto extends IObjectState {
    name: string;

    orderGroupServices: OrderGroupServiceUpsertDto[];
    items: ItemUpsertDto[];
}


