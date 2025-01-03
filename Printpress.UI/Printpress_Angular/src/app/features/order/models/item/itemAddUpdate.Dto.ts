import { iObjectState } from "../../../../core/models/iObjectState";


export interface itemAddUpdateDto extends iObjectState{
    name:string;
    quantity:number;
    price:number;
}
