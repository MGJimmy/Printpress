import { IObjectState } from "../../../../core/models/i-object-state";
import { OrderGroupUpsertDto } from "../orderGroup/order-group-upsert.Dto";

export class OrderUpsertDto extends IObjectState {

    constructor(public id: number, public name: string, public clientId: number, public orderGroups: OrderGroupUpsertDto[]) {
        super();
        console.log(orderGroups);

    }
}
