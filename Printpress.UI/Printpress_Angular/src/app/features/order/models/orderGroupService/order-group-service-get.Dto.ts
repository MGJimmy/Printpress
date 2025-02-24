import { IObjectState } from "../../../../core/models/i-object-state";

export interface OrderGroupServiceGetDto extends IObjectState {
    id: number;
    serviceId: number;
    orderGroupId: number;
    serviceName?: string;
}