import { itemAddUpdateDto } from "../item/itemAddUpdate.Dto";
import { orderGroupServiceAddUpdateDto } from "../orderGroupService/orderGroupServiceAddUpdate.Dto";

export interface orderGroupGetDto {
    id:number;
    orderId:number;
    deliveryDate?:Date;
    deliveryName?:string
    receiverName?:string
    name: string;

    orderGroupServices: orderGroupServiceAddUpdateDto[];
    items: itemAddUpdateDto[];
}