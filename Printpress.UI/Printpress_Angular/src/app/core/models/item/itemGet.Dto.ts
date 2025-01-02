import { itemAddUpdateDto } from "./itemAddUpdate.Dto";

export interface itemGetDto extends itemAddUpdateDto{
    id:number;
    groupId: number;
}
