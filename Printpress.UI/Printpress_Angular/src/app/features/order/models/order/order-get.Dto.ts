import { OrderGroupUpsertDto } from "../orderGroup/order-group-upsert.Dto";

export interface OrderGetDto {
    id:number;
    totalPrice:number;
    totalPaid:number;
    name:string;
    clientId:number;

    orderGroups: OrderGroupUpsertDto[];
}