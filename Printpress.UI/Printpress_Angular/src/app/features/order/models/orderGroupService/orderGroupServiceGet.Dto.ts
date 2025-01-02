import { orderGroupServiceAddUpdateDto } from "./orderGroupServiceAddUpdate.Dto";


export interface orderGroupServiceGetDto extends orderGroupServiceAddUpdateDto{
    id:number;
    OrderId:number;
}