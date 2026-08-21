import { IObjectState } from "../../../../core/models/i-object-state";

export interface OrderGroupServiceUpsertDto extends IObjectState {
    id: string;
    ServiceId: string;
    IsCover: boolean;
}


