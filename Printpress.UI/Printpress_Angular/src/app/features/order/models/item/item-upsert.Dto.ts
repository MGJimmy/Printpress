import { IObjectState } from "../../../../core/models/i-object-state";


export interface ItemUpsertDto extends IObjectState{
    name:string;
    quantity:number;
    price:number;
}
