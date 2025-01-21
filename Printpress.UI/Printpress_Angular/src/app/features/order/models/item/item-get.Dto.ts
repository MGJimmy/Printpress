import { IObjectState } from "../../../../core/models/i-object-state";

export interface ItemGetDto extends IObjectState {
    id:number;
    name:string;
    quantity:number;
    price:number;
    groupId: number;
}