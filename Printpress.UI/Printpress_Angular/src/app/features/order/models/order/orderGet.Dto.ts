import { orderGroupAddUpdateDto } from "../orderGroup/orderGroupAddUpdate.Dto";

export interface orderGetDto {
    id:number;
    totalPrice:number;
    totalPaid:number;
    name:string;
    clientId:number;

    orderGroups: orderGroupAddUpdateDto[];
}