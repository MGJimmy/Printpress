import { IObjectState } from "../../../../core/models/i-object-state";
import { ItemGetDto } from "../item/item-get.Dto";
import { OrderGroupServiceGetDto } from "../orderGroupService/order-group-service-get.Dto";


export interface OrderGroupGetDto extends IObjectState {
    id: number;
    name: string;
    deliveryDate?: Date;
    deliveredFrom?: string;
    deliveredTo?: string;
    deliveryNotes?: string;
    status?:string;
    orderId: number;
    deliveryName?: string
    receiverName?: string
    isHasPrintingService: boolean;
    isHasSellingService: boolean;
    isHasStaplingService: boolean;

    orderGroupServices: OrderGroupServiceGetDto[];
    items: ItemGetDto[];
}