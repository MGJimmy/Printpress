import { IObjectState } from "../../../../core/models/i-object-state";

export interface OrderServicesGetDTO extends IObjectState{

    id: string;
    // orderId: string;
    serviceId: string;
    price: number;
}
