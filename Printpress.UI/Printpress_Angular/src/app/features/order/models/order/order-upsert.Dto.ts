import { IObjectState } from "../../../../core/models/i-object-state";
import { OrderGroupUpsertDto } from "../orderGroup/order-group-upsert.dto";

export interface OrderUpsertDto  extends IObjectState{
    name:string;
    clientId:number;

    orderGroups: OrderGroupUpsertDto[];
}
