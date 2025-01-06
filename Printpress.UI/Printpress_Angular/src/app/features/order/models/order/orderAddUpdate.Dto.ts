import { iObjectState } from "../../../../core/models/iObjectState";
import { orderGroupAddUpdateDto } from "../orderGroup/orderGroupAddUpdate.Dto";

export interface orderAddUpdateDto  extends iObjectState{
    name:string;
    clientId:number;

    orderGroups: orderGroupAddUpdateDto[];
}
