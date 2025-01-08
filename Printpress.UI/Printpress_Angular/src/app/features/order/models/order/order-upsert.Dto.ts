import { IObjectState } from "../../../../core/models/iObjectState";
import { OrderGroupUpsertDto } from "../orderGroup/order-group-upsert.Dto";

export interface OrderUpsertDto  extends IObjectState{
    name:string;
    clientId:number;

    orderGroups: OrderGroupUpsertDto[];
}
