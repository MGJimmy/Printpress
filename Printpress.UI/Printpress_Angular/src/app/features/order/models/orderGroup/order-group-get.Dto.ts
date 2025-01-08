import { ItemUpsertDto } from "../item/item-upsert.Dto";
import { OrderGroupServiceUpsertDto } from "../orderGroupService/order-group-service-upsert.Dto";

export interface OrderGroupGetDto {
    id:number;
    orderId:number;
    deliveryDate?:Date;
    deliveryName?:string
    receiverName?:string
    name: string;

    orderGroupServices: OrderGroupServiceUpsertDto[];
    items: ItemUpsertDto[];
}