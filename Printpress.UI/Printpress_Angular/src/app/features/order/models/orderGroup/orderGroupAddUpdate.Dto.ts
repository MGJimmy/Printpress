import { iObjectState } from "../../../../core/models/iObjectState";
import { itemAddUpdateDto } from "../item/itemAddUpdate.Dto";
import { orderGroupServiceAddUpdateDto } from "../orderGroupService/orderGroupServiceAddUpdate.Dto";

export interface orderGroupAddUpdateDto extends iObjectState {
    name: string;

    orderGroupServices: orderGroupServiceAddUpdateDto[];
    items: itemAddUpdateDto[];
}


