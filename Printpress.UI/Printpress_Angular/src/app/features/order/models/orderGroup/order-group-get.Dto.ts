import { ItemGetDto } from "../item/item-get.Dto";
import { OrderGroupServiceGetDto } from "../orderGroupService/order-group-service-get.Dto";


export interface OrderGroupGetDto {
    id:number;
    orderId:number;
    deliveryDate?:Date;
    deliveryName?:string
    receiverName?:string
    name: string;

    orderGroupServices: OrderGroupServiceGetDto[];
    items: ItemGetDto[];
}