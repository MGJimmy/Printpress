import { IObjectState } from "../../../../core/models/iObjectState";


export interface ItemUpsertDto extends IObjectState{
    name:string;
    quantity:number;
    price:number;
}
