import { OrderGroupGetDto } from "../orderGroup/order-group-get.Dto";

export interface OrderGetDto {
    id:number;
    totalPrice:number;
    totalPaid:number;
    name:string;
    clientId:number;

    orderGroups: OrderGroupGetDto[];
}