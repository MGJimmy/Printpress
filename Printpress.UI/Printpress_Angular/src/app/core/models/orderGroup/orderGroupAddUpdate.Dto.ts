import { itemAddUpdateDto } from "../item/itemAddUpdate.Dto";
import { orderAddUpdateDto } from "../order/orderAddUpdate.Dto";
import { orderGroupServiceAddUpdateDto } from "../orderGroupService/orderGroupServiceAddUpdate.Dto";

export interface orderGroupAddUpdateDto{
    name:string;

    orderGroupServices: orderGroupServiceAddUpdateDto[];
    items:itemAddUpdateDto[];
}


