import { IObjectState } from "../../../../core/models/i-object-state";
import { ItemGetDto } from "../item/item-get.Dto";
import { OrderGroupServiceGetDto } from "../orderGroupService/order-group-service-get.Dto";


export interface OrderGroupGetDto extends IObjectState {
    id: number;
    orderId: number;
    deliveryDate?: Date;
    deliveryName?: string
    receiverName?: string
    name: string;
    isHasPrintingService: boolean;
    isHasSellingService: boolean;
    isHasStaplingService: boolean;

    orderGroupServices: OrderGroupServiceGetDto[];
    items: ItemGetDto[];
}