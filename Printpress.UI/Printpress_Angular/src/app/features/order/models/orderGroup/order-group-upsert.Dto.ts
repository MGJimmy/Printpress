import { IObjectState } from "../../../../core/models/iObjectState";
import { ItemUpsertDto } from "../item/item-upsert.Dto";
import { OrderGroupServiceUpsertDto } from "../orderGroupService/order-group-service-upsert.Dto";

export interface OrderGroupUpsertDto extends IObjectState {
    name: string;

    orderGroupServices: OrderGroupServiceUpsertDto[];
    items: ItemUpsertDto[];
}


