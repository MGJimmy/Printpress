import { IObjectState } from "../../../../core/models/i-object-state";

export interface OrderGroupServiceGetDto extends IObjectState {
    id: string;
    serviceId: string;
    orderGroupId: string;
    serviceName?: string;
}