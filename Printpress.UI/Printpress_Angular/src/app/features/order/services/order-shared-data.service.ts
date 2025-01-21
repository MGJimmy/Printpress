import { Injectable } from '@angular/core';
import { OrderUpsertDto } from '../models/order/order-upsert.Dto';
import { OrderEntity } from '../models/order/order.entity';
import { OrderGetDto } from '../models/order/order-get.Dto';
import { OrderGroupGetDto } from '../models/orderGroup/order-group-get.Dto';
import { OrderGroupServiceGetDto } from '../models/orderGroupService/order-group-service-get.Dto';
import { ItemGetDto } from '../models/item/item-get.Dto';
import { ItemUpsertDto } from '../models/item/item-upsert.Dto';
import { ObjectStateEnum } from '../../../core/models/object-state.enum';

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
  public intializeNewGroup(): number {
    let tempId: number = this.generateTempId(this.orderObject.orderGroups.map(x => x.id));

    let orderGroup: OrderGroupGetDto = {
      id: tempId,
      orderId: this.orderObject.id,
      name: '',
      orderGroupServices: [],
      items: []
    };

    this.orderObject.orderGroups.push(orderGroup);

    return tempId
  }

  public updateOrderGroup(id: number, name: string, groupServices: OrderGroupServiceGetDto[], groupItmes: ItemGetDto[]) {

    let orderGroup: OrderGroupGetDto = {
      id: id,
      orderId: this.orderObject.id,
      name: name,
      orderGroupServices: groupServices,
      items: groupItmes
    };

    let currentGroup = this.orderObject.orderGroups.find(x => x.id == id);
    currentGroup = orderGroup;
  }

  public getOrderGroup(id: number): OrderGroupGetDto {
    let group = this.orderObject.orderGroups.find(x => x.id == id);
    if (group) {
      return group;
    } else {
      throw 'cannot find a group with id = ' + id;
    }
  }
  
  /**
   * Returns temp id of the added object.
   * 
   */
  public addOrderGroupService(orderGroupId: number, ServiceId: number): number {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if (orderGroup === undefined) {
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
 * Returns temp id of the generated object.
 * 
 */
  public initializeTempItem(orderGroupId: number): ItemGetDto {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if (orderGroup === undefined) {
      throw new Error('Order group not found');
    }

    let tempId = this.generateTempId(orderGroup.items.map(x => x.id));

    let item: ItemGetDto = {
      id: tempId,
      name: '',
      quantity: 0,
      price: 0,
      groupId: orderGroupId,
      objectState: ObjectStateEnum.temp
    }

    orderGroup.items.push(item);

    return item;

  }

  public addItem(orderGroupId: number, itemId: number, name: string, quantity: number, price: number): void {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if (orderGroup === undefined) {
      throw new Error('Order group not found');
    }

    let item : ItemGetDto = orderGroup.items.find(x => x.id == itemId)!;

    item.name = name;
    item.quantity = quantity;
    item.price = price;
    item.objectState = ObjectStateEnum.added;
  }

  public updateItem(orderGroupId: number, itemId: number, name: string, quantity: number, price: number): void {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if (orderGroup === undefined) {
      throw new Error('Order group not found');
    }

    let item : ItemGetDto = orderGroup.items.find(x => x.id == itemId)!;

    item.name = name;
    item.quantity = quantity;
    item.price = price;
    item.objectState = ObjectStateEnum.updated;
  }

  public getOrderObject(): OrderGetDto {
    return this.orderObject;
  }

  public getOrderGroupServices(orderGroupId: number): OrderGroupServiceGetDto[] {
    return this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.orderGroupServices;
  }

  public getItem(orderGroupId: number, itemId: number): ItemGetDto {
    return this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.items.find(x => x.id === itemId)!;
  }



  private generateTempId(ids: number[]): number {
    if (ids.length === 0) {
      return 1;
    }

    return Math.max(...ids) + 1;
  }
}
