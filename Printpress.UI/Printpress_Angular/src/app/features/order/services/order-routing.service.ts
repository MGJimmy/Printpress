import { Injectable } from '@angular/core';
import { ORDER_ROUTES } from '../constants/order-routes.constants';

@Injectable({
    providedIn: 'root'
})
export class OrderRoutingService {

    public getOrderListRoute(): string {
        return `/${ORDER_ROUTES.LIST}`;
    }

    public getOrderAddRoute(): string {
        return `/${ORDER_ROUTES.ORDER.BASE}/${ORDER_ROUTES.ORDER.ADD}`;
    }

    public getOrderEditRoute(orderId: number): string {
        return `/${ORDER_ROUTES.ORDER.BASE}/${ORDER_ROUTES.ORDER.EDIT.replace(':id', orderId.toString())}`;
    }

    public getOrderViewRoute(orderId: number): string {
        return `/${ORDER_ROUTES.ORDER.BASE}/${ORDER_ROUTES.ORDER.VIEW.replace(':id', orderId.toString())}`;
    }

    public getGroupRoute(groupId: number): string {
        return `/${ORDER_ROUTES.ORDER.BASE}/${ORDER_ROUTES.ORDER.GROUP.EDIT.replace(':id', groupId.toString())}`;
    }

    public getGroupAddRoute(): string {
        return `/${ORDER_ROUTES.ORDER.BASE}/${ORDER_ROUTES.ORDER.GROUP.ADD}`;
    }

    public getItemAddRoute(groupId: number): string {
        return `/${ORDER_ROUTES.ORDER.BASE}/${ORDER_ROUTES.ORDER.ITEM.ADD.replace(':groupId', groupId.toString())}`;
    }

    public getItemEditRoute(groupId: number, itemId: number): string {
        return `/${ORDER_ROUTES.ORDER.BASE}/${ORDER_ROUTES.ORDER.ITEM.EDIT
            .replace(':groupId', groupId.toString())
            .replace(':id', itemId.toString())}`;
    }
}