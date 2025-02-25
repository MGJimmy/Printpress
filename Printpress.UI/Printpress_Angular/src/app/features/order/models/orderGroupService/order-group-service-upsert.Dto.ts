import { IObjectState } from "../../../../core/models/i-object-state";

export interface OrderGroupServiceUpsertDto extends IObjectState {
    id: number;
    ServiceId: number;
}


