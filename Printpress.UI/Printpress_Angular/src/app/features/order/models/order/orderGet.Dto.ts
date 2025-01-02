import { orderAddUpdateDto } from "./orderAddUpdate.Dto";

export interface orderGetDto extends orderAddUpdateDto{
    id:number;
    totalPrice:number;
    totalPaid:number;
}