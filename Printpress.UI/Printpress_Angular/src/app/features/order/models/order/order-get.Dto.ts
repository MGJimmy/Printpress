import { IObjectState } from "../../../../core/models/i-object-state";
import { OrderServicesGetDTO } from "../order-service/order-service-getDto";
import { OrderGroupGetDto } from "../orderGroup/order-group-get.Dto";
import { OrderSellingItemGetDto } from "../orderSellingItem/order-selling-item-get.dto";

export interface OrderGetDto extends IObjectState {
    id:string;
    totalPrice:number;
    totalPaid:number;
    name:string;
    clientId:string;
    clientName:string,
    status :string,
    orderGroups: OrderGroupGetDto[];
    orderServices: OrderServicesGetDTO[];
    sellingItems: OrderSellingItemGetDto[];
}