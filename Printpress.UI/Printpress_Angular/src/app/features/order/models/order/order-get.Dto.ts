import { IObjectState } from "../../../../core/models/i-object-state";
import { OrderServicesGetDTO } from "../order-service/order-service-getDto";
import { OrderGroupGetDto } from "../orderGroup/order-group-get.Dto";

export interface OrderGetDto extends IObjectState {
    id:number;
    totalPrice:number;
    totalPaid:number;
    name:string;
    clientId:number;
    clientName:string,
    status :string,
    orderGroups: OrderGroupGetDto[];
    orderServices: OrderServicesGetDTO[];
}