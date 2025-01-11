import { Injectable } from '@angular/core';
import { OrderUpsertDto } from '../models/order/order-upsert.Dto';
import { OrderEntity } from '../models/order/order.entity';
import { OrderGetDto } from '../models/order/order-get.Dto';

@Injectable()
export class OrderSharedDataService {

  private orderObject!: OrderGetDto;

  constructor() {
    this.initializeOrderObject();
  }

  private initializeOrderObject(): void {
    //this.orderObject = new OrderUpsertDto('', 0, []);
  }

  private setOrderObject(order: OrderUpsertDto): void {
    //this.orderObject = order;
  }

  public getOrderGroup() {

  }
}
