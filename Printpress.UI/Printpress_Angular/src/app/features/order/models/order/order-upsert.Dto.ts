import { IObjectState } from "../../../../core/models/i-object-state";
import { OrderServicesUpsertDTO } from "../order-service/order-service-upsertDto";
import { OrderGroupUpsertDto } from "../orderGroup/order-group-upsert.Dto";

export class OrderUpsertDto extends IObjectState {

    constructor(public id: number,
        public name: string,
        public clientId: number,
        public orderGroups: OrderGroupUpsertDto[],
        public orderServices: OrderServicesUpsertDTO[]) {

        super();
        console.log(orderGroups);

    }
}
