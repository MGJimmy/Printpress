import { IObjectState } from "../../../../core/models/i-object-state";
import { OrderGroupGetDto } from "../orderGroup/order-group-get.Dto";

export interface OrderGetDto extends IObjectState {
    id:number;
    totalPrice:number;
    totalPaid:number;
    name:string;
    clientId:number;

    orderGroups: OrderGroupGetDto[];
}