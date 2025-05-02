import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class OrderRoutingService {


    public getOrderListRoute(): string {
        return '/orderlist';
    }

    public getOrderAddRoute(): string {
        return '/order/add';
    }

    public getOrderEditRoute(orderId: number): string {
        return `/order/edit/${orderId}`;
    }

    public getOrderViewRoute(orderId: number): string {
        return `/order/view/${orderId}`;
    }

    public getGroupRoute(groupId: number): string {
        return `/order/group/${groupId}`;
    }

    public getGroupAddRoute(): string {
        return '/order/group';
    }

    public getItemAddRoute(groupId: number): string {
        return `/order/item/add/${groupId}`;
    }

    public getItemEditRoute(groupId: number, itemId: number): string {
        return `/order/item/edit/${groupId}/${itemId}`;
    }
}