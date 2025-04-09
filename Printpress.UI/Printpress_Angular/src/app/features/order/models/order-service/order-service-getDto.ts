import { IObjectState } from "../../../../core/models/i-object-state";

export interface OrderServicesGetDTO extends IObjectState{

    id: number;
    // orderId: number;
    serviceId: number;
    price: number;
}
