import { ItemGetDto } from "./item/item-get.Dto";
import { ItemUpsertDto } from "./item/item-upsert.Dto";
import { OrderServicesGetDTO } from "./order-service/order-service-getDto";
import { OrderServicesUpsertDTO } from "./order-service/order-service-upsertDto";
import { OrderGetDto } from "./order/order-get.Dto";
import { OrderUpsertDto } from "./order/order-upsert.Dto";
import { OrderGroupGetDto } from "./orderGroup/order-group-get.Dto";
import { OrderGroupUpsertDto } from "./orderGroup/order-group-upsert.Dto";

function mapOrderGetDtoToUpsertDto(order: OrderGetDto): OrderUpsertDto {
    return new OrderUpsertDto(
        order.id,
        order.name,
        order.clientId,
        order.orderGroups.map(group => mapOrderGroupGetDtoToUpsertDto(group)),
        order.orderServices.map(service => mapOrderServiceGetDtoToUpsertDto(service))
    );
}

function mapOrderGroupGetDtoToUpsertDto(group: OrderGroupGetDto): OrderGroupUpsertDto {
    return new OrderGroupUpsertDto(
        group.id,
        group.orderId,
        group.name,
        group.deliveryDate,
        group.deliveryName,
        group.receiverName,
        group.isHasPrintingService,
        group.isHasSellingService,
        group.orderGroupServices.map(service => ({
            id: service.id,
            serviceId: service.serviceId,
            orderGroupId: service.orderGroupId,
            serviceName: service.serviceName
        })),
        group.items.map(item => mapItemGetDtoToUpsertDto(item))
    );
}

function mapOrderServiceGetDtoToUpsertDto(service: OrderServicesGetDTO): OrderServicesUpsertDTO {
    return {
        id: service.id,
        serviceId: service.serviceId,
        price: service.price
    };
}

function mapItemGetDtoToUpsertDto(item: ItemGetDto): ItemUpsertDto {
    return {
        id: item.id,
        name: item.name,
        quantity: item.quantity,
        price: item.price,
        groupId: item.groupId,
        itemDetails: item.itemDetails.map(detail => ({
            ...detail
        }))
    };
}

// Let me know if you want me to define the Upsert DTO classes or adjust anything! 🚀
