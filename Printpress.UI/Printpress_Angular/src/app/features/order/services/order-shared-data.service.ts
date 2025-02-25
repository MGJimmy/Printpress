import { Injectable } from '@angular/core';
import { OrderGetDto } from '../models/order/order-get.Dto';
import { OrderGroupGetDto } from '../models/orderGroup/order-group-get.Dto';
import { OrderGroupServiceGetDto } from '../models/orderGroupService/order-group-service-get.Dto';
import { ItemGetDto } from '../models/item/item-get.Dto';
import { ObjectStateEnum } from '../../../core/models/object-state.enum';
import { OrderServicesGetDTO } from '../models/order-service/order-service-getDto';
import { ServiceService } from '../../setup/services/service.service';
import { ServiceCategoryEnum } from '../../setup/models/service-category.enum';

/*
  Notes: 
    * Return new object when returning object from the service to prevent changing the original object. using spread operator. {...object}
*/


@Injectable()
export class OrderSharedDataService {

  private orderObject!: OrderGetDto;

  constructor(private serviceService:ServiceService) {
    this.initializeOrderObject();
  }



  /*
    =======================
    Start order object methods
    =======================
  */

  private initializeOrderObject(): void {
    this.orderObject = {
      id: 0,
      totalPrice: 0,
      totalPaid: 0,
      name: '',
      clientId: 0,
      objectState: ObjectStateEnum.temp,
      orderGroups: [],
      orderServices: []
    };
  }



  public setOrderObject(order: OrderGetDto): void {
    this.orderObject = order;
  }

  public getOrderObject(): OrderGetDto {
    return this.orderObject;
  }

  public getOrderPageRoute(): string {
    if (this.orderObject.objectState == ObjectStateEnum.temp) {
      return '/order/add';
    } else {
      return '/order/edit/' + this.orderObject.id;
    }
  }

  public getOrderListRoute(): string {
    return '/order';
  }
  /*
    =======================
    End order object methods
    =======================
  */













  /*
    =======================
    Start order group methods
    =======================
  */

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
      isHasPrintingService: false,
      isHasSellingService: false,
      orderGroupServices: [],
      items: [],
      objectState: ObjectStateEnum.temp
    };

    this.orderObject.orderGroups.push(orderGroup);

    return tempId
  }

  public getOrderGroup(id: number): OrderGroupGetDto {
    let group = this.orderObject.orderGroups.find(x => x.id == id);
    if (group) {
      return group;
    } else {
      throw 'cannot find a group with id = ' + id;
    }
  }

  public updateOrderGroup(id: number, name: string, groupServices: OrderGroupServiceGetDto[]) {
    let orderGroup = this.getOrderGroup(id);

    orderGroup.name = name;
    orderGroup.orderGroupServices = groupServices;
    orderGroup.objectState = ObjectStateEnum.updated;
  }

  public saveNewOrderGroup(id: number, name: string, groupServices: OrderGroupServiceGetDto[]) {
    let orderGroup = this.getOrderGroup(id);

    orderGroup.name = name;
    orderGroup.orderGroupServices = groupServices;
    orderGroup.objectState = ObjectStateEnum.added;
  }

  public deleteNewlyAddedGroup(groupId: number) {
    let orderGroups = this.getOrderObject().orderGroups;

    const index = orderGroups.findIndex(x => x.id === groupId);
    if (index !== -1) {
      orderGroups.splice(index, 1);
    }
  }

  public deleteExistingGroup(groupId: number) {
    let group = this.getOrderGroup(groupId);
    group.objectState = ObjectStateEnum.deleted;
  }

  public setGroupBooleanProperty(groupId: number, isHasPrintingService: boolean, isHasSellingService: boolean) {
    let group = this.getOrderGroup(groupId);

    group.isHasPrintingService = isHasPrintingService;
    group.isHasSellingService = isHasSellingService;
  }

  /*
    =======================
    End order group methods
    =======================
  */












  /*
   =======================
   Start item methods
   =======================
 */

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

    // check if group service contains printing service then init the two item details for printing with default values
    // to set them in the form

    let item: ItemGetDto = {
      id: tempId,
      name: '',
      quantity: 0,
      price: 0,
      groupId: orderGroupId,
      itemDetails: [],
      objectState: ObjectStateEnum.temp
    }

    orderGroup.items.push(item);

    return { ...item } as ItemGetDto;

  }

  public addItem(orderGroupId: number, itemId: number, name: string, quantity: number, price: number): void {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if (orderGroup === undefined) {
      throw new Error('Order group not found');
    }

    let item: ItemGetDto = orderGroup.items.find(x => x.id == itemId)!;

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

    let item: ItemGetDto = orderGroup.items.find(x => x.id == itemId)!;

    item.name = name;
    item.quantity = quantity;
    item.price = price;
    item.objectState = ObjectStateEnum.updated;
  }

  public getItem(orderGroupId: number, itemId: number): ItemGetDto {
    let item = this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.items.find(x => x.id === itemId)!;

    return { ...item } as ItemGetDto;

  }

  public deleteNewlyAddedItem(groupId: number, itemId: number) {
    let groupItems = this.getOrderGroup(groupId).items;

    const index = groupItems.findIndex(x => x.id === itemId);
    if (index !== -1) {
      groupItems.splice(index, 1);
    }
  }

  public deleteExistingItem(groupId: number, itemId: number) {
    let item = this.getItem(groupId, itemId);
    item.objectState = ObjectStateEnum.deleted;
  }

  /*
    =======================
    End item methods
    =======================
  */











  /*
    =======================
    Start group services methods
    =======================
  */

  public getOrderGroupServices(orderGroupId: number): OrderGroupServiceGetDto[] {
    return this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.orderGroupServices;
  }

  public getAllOrderGroupsServices(): OrderGroupServiceGetDto[] {
    return this.orderObject.orderGroups.flatMap(x => x.orderGroupServices);
  }

  public addOrderGroupService(orderGroupId: number, ServiceId: number): number {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if (orderGroup === undefined) {
      throw new Error('Order group not found');
    }


    let tempId = this.generateTempId(orderGroup.orderGroupServices.map(x => x.id));

    let orderGroupService: OrderGroupServiceGetDto = {
      id: tempId,
      orderGroupId: orderGroupId,
      serviceId: ServiceId,
      objectState : ObjectStateEnum.temp
    };

    orderGroup.orderGroupServices.push(orderGroupService);

    this.checkGroupServicesCategory(orderGroup);

    return tempId;
  }

  private checkGroupServicesCategory(group:OrderGroupGetDto){
      this.serviceService.getServices(group.orderGroupServices.map(s => s.serviceId)).subscribe(services =>{
      group.isHasPrintingService = services.some(s => s.serviceCategory === ServiceCategoryEnum.Printing);
      group.isHasSellingService = services.some(s => s.serviceCategory === ServiceCategoryEnum.Selling);
    });
  }

  public deleteGroupService(groupId:number, serviceId:number){
    let groupServices = this.orderObject.orderGroups.find(x => x.id = groupId)!.orderGroupServices
    
    
    let groupService = groupServices.find(s => s.id = serviceId)!;

    if(groupService?.objectState === ObjectStateEnum.temp){
      let index = groupServices.findIndex(s => s.id = serviceId);
      groupServices.splice(index, 1);
    }
    else{
      groupService.objectState = ObjectStateEnum.deleted;
    }


  }

  /*
  =======================
  End group services methods
  =======================
*/







  /*
    =======================
    Start order services methods
    =======================
  */

  // public getOrderServices(): OrderServicesGetDTO[] {
  //   return this.orderObject.orderServices;
  // }

  public setOrderServices(orderServices: OrderServicesGetDTO[]) {
    return this.orderObject.orderServices = orderServices;
  }

  public addOrderServicesDistinct(orderService: OrderServicesGetDTO) {
    if (!this.orderObject.orderServices.find(x => x.serviceId == orderService.serviceId)) {
      this.orderObject.orderServices.push(orderService);
    }
  }


  /*
  =======================
  End order services methods
  =======================
*/



  private generateTempId(ids: number[]): number {
    if (ids.length === 0) {
      return 1;
    }

    return Math.max(...ids) + 1;
  }
}
