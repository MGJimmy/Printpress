import { IObjectState } from "../../../../core/models/i-object-state";

export interface OrderServicesUpsertDTO extends IObjectState{

    id: number;
    serviceId: number;
    price: number;
}