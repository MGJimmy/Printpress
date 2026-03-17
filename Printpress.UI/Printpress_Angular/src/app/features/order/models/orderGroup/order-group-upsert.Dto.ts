import { IObjectState } from "../../../../core/models/i-object-state";
import { ItemUpsertDto } from "../item/item-upsert.Dto";
import { OrderGroupServiceUpsertDto } from "../orderGroupService/order-group-service-upsert.Dto";

export interface OrderGroupUpsertDto extends IObjectState {
    id: string;
    name: string;
    executionType: string;
    orderGroupServices: OrderGroupServiceUpsertDto[];
    items: ItemUpsertDto[];
}


