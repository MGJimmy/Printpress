import { orderGroupAddUpdateDto } from "./orderGroupAddUpdate.Dto";

export interface orderGroupGetDto extends orderGroupAddUpdateDto{
    id:number;
    orderId:number;
    deliveryDate?:Date;
    deliveryName?:string
    receiverName?:string
}