import { orderGroupAddUpdateDto } from "../orderGroup/orderGroupAddUpdate.Dto";

export interface orderAddUpdateDto{
    name:string;
    clientId:number;

    orderGroups: orderGroupAddUpdateDto[]
}
