import { IObjectState } from "../../../../core/models/i-object-state";
import { OrderServicesUpsertDTO } from "../order-service/order-service-upsertDto";
import { OrderGroupUpsertDto } from "../orderGroup/order-group-upsert.Dto";
import { OrderSellingItemUpsertDto } from "../orderSellingItem/order-selling-item-upsert.dto";

export class OrderUpsertDto extends IObjectState {

    constructor(
        public id: string,
        public name: string,
        public clientId: string,
        public orderGroups: OrderGroupUpsertDto[],
        public orderServices: OrderServicesUpsertDTO[],
        public sellingItems: OrderSellingItemUpsertDto[]) {

        super();
    }
}
