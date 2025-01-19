import { Injectable } from '@angular/core';
import { OrderUpsertDto } from '../models/order/order-upsert.Dto';
import { OrderEntity } from '../models/order/order.entity';
import { OrderGetDto } from '../models/order/order-get.Dto';
import { OrderGroupGetDto } from '../models/orderGroup/order-group-get.Dto';
import { OrderGroupServiceGetDto } from '../models/orderGroupService/order-group-service-get.Dto';
import { ItemGetDto } from '../models/item/item-get.Dto';
import { ItemUpsertDto } from '../models/item/item-upsert.Dto';

@Injectable()
export class OrderSharedDataService {

  private orderObject!: OrderGetDto;

  constructor() {
    this.initializeOrderObject();
  }

  private initializeOrderObject(): void {
    this.orderObject = {
      id: 0,
      totalPrice: 0,
      totalPaid: 0,
      name: '',
      clientId: 0,
      orderGroups: []
    };
  }

  public setOrderObject(order: OrderGetDto): void {
    this.orderObject = order;
  }

  /**
   * Returns temp id of the added object.
   * 
   */
  public addOrderGroup(name:string): number {
    let tempId:number = this.generateTempId(this.orderObject.orderGroups.map(x => x.id));

    let orderGroup: OrderGroupGetDto = {
      id: tempId,
      orderId: this.orderObject.id,
      name: name,
      orderGroupServices: [],
      items: []
    };

    this.orderObject.orderGroups.push(orderGroup);

    return tempId
  }

  /**
   * Returns temp id of the added object.
   * 
   */
  public addOrderGroupService(orderGroupId: number, ServiceId:number): number {
    let orderGroup: OrderGroupGetDto| undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if(orderGroup === undefined) {
      throw new Error('Order group not found');
    }
    let tempId = this.generateTempId(orderGroup.orderGroupServices.map(x => x.id));

    let orderGroupService: OrderGroupServiceGetDto = {
      id: tempId,
      OrderGroupId: orderGroupId,
      ServiceId: ServiceId,
    };

    orderGroup.orderGroupServices.push(orderGroupService);

    return tempId;
  }

  /**
   * Returns temp id of the added object.
   * 
   */
  public addItem(orderGroupId: number, name:string, quantity:number, price:number): number {
    let orderGroup: OrderGroupGetDto| undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if(orderGroup === undefined) {
      throw new Error('Order group not found');
    }

    let tempId = this.generateTempId(orderGroup.items.map(x => x.id));
    let item: ItemGetDto = {
      id: tempId,
      name: name,
      quantity: quantity,
      price: price,
      groupId: orderGroupId
    }

    orderGroup.items.push(item);

    return tempId;
  }



  public getOrderObject(): OrderGetDto {
    return this.orderObject;
  }

  public getOrderGroup(id:number): OrderGroupGetDto {
    return this.orderObject.orderGroups.find(x => x.id === id)!;
  }

  public getOrderGroupServices(orderGroupId:number): OrderGroupServiceGetDto[] {
    return this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.orderGroupServices;
  }

  public getItem(orderGroupId:number, itemId:number): ItemGetDto {
    return this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.items.find(x => x.id === itemId)!;
  }


    
  private generateTempId(ids:number[]): number {
    if(ids.length === 0) {
      return 1;
    }

    return Math.max(...ids) + 1;
  }
}
