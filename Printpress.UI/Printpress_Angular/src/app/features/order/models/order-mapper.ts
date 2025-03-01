import { ItemGetDto } from "./item/item-get.Dto";
import { ItemUpsertDto } from "./item/item-upsert.Dto";
import { OrderServicesGetDTO } from "./order-service/order-service-getDto";
import { OrderServicesUpsertDTO } from "./order-service/order-service-upsertDto";
import { OrderGetDto } from "./order/order-get.Dto";
import { OrderUpsertDto } from "./order/order-upsert.Dto";
import { OrderGroupGetDto } from "./orderGroup/order-group-get.Dto";
import { OrderGroupUpsertDto } from "./orderGroup/order-group-upsert.Dto";
import { OrderGroupServiceGetDto } from "./orderGroupService/order-group-service-get.Dto";
import { OrderGroupServiceUpsertDto } from "./orderGroupService/order-group-service-upsert.Dto";

export function mapOrderGetToUpsert(orderGet: OrderGetDto): OrderUpsertDto {
    return new OrderUpsertDto(
        orderGet.id,
        orderGet.name,
        orderGet.clientId,
        orderGet.orderGroups.map(mapOrderGroupGetToUpsert),
        orderGet.orderServices.map(mapOrderServiceGetToUpsert)
    );
}

function mapOrderGroupGetToUpsert(groupGet: OrderGroupGetDto): OrderGroupUpsertDto {
    return {
        id: groupGet.id,
        name: groupGet.name,
        objectState: groupGet.objectState, // Keep object state
        orderGroupServices: groupGet.orderGroupServices.map(mapOrderGroupServiceGetToUpsert),
        items: groupGet.items.map(mapItemGetToUpsert)
    };
}

function mapOrderGroupServiceGetToUpsert(serviceGet: OrderGroupServiceGetDto): OrderGroupServiceUpsertDto {
    return {
        id: serviceGet.id,
        ServiceId: serviceGet.serviceId,
        objectState: serviceGet.objectState
    };
}

function mapItemGetToUpsert(itemGet: ItemGetDto): ItemUpsertDto {
    return {
        id: itemGet.id,
        name: itemGet.name,
        quantity: itemGet.quantity,
        price: itemGet.price,
        objectState: itemGet.objectState
    };
}

function mapOrderServiceGetToUpsert(serviceGet: OrderServicesGetDTO): OrderServicesUpsertDTO {
    return {
        id: serviceGet.id,
        serviceId: serviceGet.serviceId,
        price: serviceGet.price
    };
}