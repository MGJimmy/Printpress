import { Injectable } from '@angular/core';
import { OrderGetDto } from '../models/order/order-get.Dto';
import { OrderGroupGetDto } from '../models/orderGroup/order-group-get.Dto';
import { OrderGroupServiceGetDto } from '../models/orderGroupService/order-group-service-get.Dto';
import { ItemGetDto } from '../models/item/item-get.Dto';
import { ObjectStateEnum } from '../../../core/models/object-state.enum';
import { OrderServicesGetDTO } from '../models/order-service/order-service-getDto';
import { ServiceService } from '../../setup/services/service.service';
import { ServiceCategoryEnum } from '../../setup/models/service-category.enum';
import { ServiceGetDto } from '../../setup/models/service-get.dto';
import { ItemDetailsGetDto } from '../models/item-details/item-details-get.dto';
import { itemDetailsKeyEnum } from '../models/enums/item-details-key.enum';
import { group } from '@angular/animations';

/*
  Notes: 
    * Return new object when returning object from the service to prevent changing the original object. using spread operator. {...object}
*/


@Injectable()
export class OrderSharedDataService {

  private orderObject!: OrderGetDto;

  constructor(private serviceService: ServiceService) {
    this.initializeOrderObject();
  }



  //=======================
  //#region Order methods
  //=======================


  private initializeOrderObject(): void {
    this.orderObject = {
      id: 0,
      totalPrice: 0,
      totalPaid: 0,
      name: '',
      clientId: 0,
      clientName: '',
      objectState: ObjectStateEnum.temp,
      orderGroups: [],
      orderServices: []
    };
  }



  public setOrderObject(order: OrderGetDto): void {
    this.orderObject = order;

    // set the flags because it used in group and item components
    this.orderObject.orderGroups.forEach(group => {
      this.updateGroupFlagsOnServicesCategories(group);
    });
  }

  public setOrderName(name: string): void {
    this.orderObject.name = name;
  }
  public setOrderClient(clientId: number): void {
    this.orderObject.clientId = clientId;
  }

  public updateOrderObjectState(): void{
    this.orderObject.objectState = this.orderObject.objectState === ObjectStateEnum.temp ? ObjectStateEnum.added : ObjectStateEnum.updated;
  }

  public getOrderObject_copy(): OrderGetDto {
    return this.deepCopy(this.orderObject);
  }

  public getOrderPageRoute(): string {
    if (this.orderObject.objectState == ObjectStateEnum.temp) {
      return '/order/add';
    } else {
      return '/order/edit/' + this.orderObject.id;
    }
  }

  public getOrderListRoute(): string {
    return '/orderlist';
  }

  //=======================
  //#endregion Order methods
  //=======================















  //=======================
  //#region Group methods
  //=======================


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
      isHasStaplingService: false,
      orderGroupServices: [],
      items: [],
      objectState: ObjectStateEnum.temp
    };

    this.orderObject.orderGroups.push(orderGroup);
    return tempId
  }

  public getOrderGroup_Copy(id: number): OrderGroupGetDto {
    return this.deepCopy(this.getOrderGroup(id));
  }

  private getOrderGroup(id: number): OrderGroupGetDto {
    let group = this.orderObject.orderGroups.find(x => x.id == id);
    if (!group) {
      throw 'cannot find a group with id = ' + id;
    }

    return group;
  }

  public updateOrderGroupName(id: number, name: string) {
    let orderGroup = this.getOrderGroup(id);
    orderGroup.name = name;
  }

  public updateOrderGroup(id: number) {
    let orderGroup = this.getOrderGroup(id);
    orderGroup.objectState = ObjectStateEnum.updated;
  }

  public saveNewOrderGroup(id: number) {
    let orderGroup = this.getOrderGroup(id);
    orderGroup.objectState = ObjectStateEnum.added;
  }

  public deleteNewlyAddedGroup(groupId: number) {
    let orderGroups = this.orderObject.orderGroups;

    const index = orderGroups.findIndex(x => x.id === groupId);
    if (index !== -1) {
      orderGroups.splice(index, 1);
    }
  }

  public deleteExistingGroup(groupId: number) {
    let group = this.getOrderGroup(groupId);
    group.objectState = ObjectStateEnum.deleted;
  }

  private updateGroupFlagsOnServicesCategories(group: OrderGroupGetDto) {
    this.serviceService.getServices(group.orderGroupServices.map(s => s.serviceId)).subscribe(services => {
      group.isHasPrintingService = services.some(s => s.serviceCategory === ServiceCategoryEnum.Printing);
      group.isHasSellingService = services.some(s => s.serviceCategory === ServiceCategoryEnum.Selling);
      group.isHasStaplingService = services.some(s => s.serviceCategory === ServiceCategoryEnum.Stapling);
    });
  }
  //=======================
  //#endregion Group methods
  //=======================













  //=======================
  //#region item methods
  //=======================


  /**
 * Returns temp id of the generated object.
 * 
 */
  public initializeTempItem(orderGroupId: number): number {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id == orderGroupId);

    if (orderGroup === undefined) {
      throw new Error('Order group not found');
    }

    let tempId = this.generateTempId(orderGroup.items.map(x => x.id));

    // check if group service contains printing service then init the two item details for printing with default values
    let itemDetails: ItemDetailsGetDto[] = [];

    if (orderGroup.isHasPrintingService) {
      itemDetails.push({
        id: 0,
        itemId: tempId,
        key: itemDetailsKeyEnum.NumberOfPages,
        value: "",
        objectState: ObjectStateEnum.temp
      });
      itemDetails.push({
        id: 1,
        itemId: tempId,
        key: itemDetailsKeyEnum.NumberOfPrintingFaces,
        value: "",
        objectState: ObjectStateEnum.temp
      });
    }

    let item: ItemGetDto = {
      id: tempId,
      name: '',
      quantity: 0,
      price: 0,
      groupId: orderGroupId,
      details: itemDetails,
      objectState: ObjectStateEnum.temp
    }

    orderGroup.items.push(item);

    return item.id;
  }

  public addItem(orderGroupId: number, itemId: number, name: string, quantity: number, price: number, numberOfPages: number | undefined, numberOfPrintingFaces: number | undefined): void {

    let orderGroup = this.orderObject.orderGroups.find(x => x.id == orderGroupId);

    if (!orderGroup) {
      throw new Error('Order group not found');
    }

    let item: ItemGetDto = orderGroup.items.find(x => x.id == itemId)!;

    item.name = name;
    item.quantity = quantity;
    item.price = price;
    item.objectState = ObjectStateEnum.added;

    if (orderGroup.isHasPrintingService) {
      if (!numberOfPages || !numberOfPrintingFaces) {
        throw new Error('Number of pages and number of printing faces are required for printing service');
      }

      let numberOfPagesItemDetail: ItemDetailsGetDto = item.details.find(x => x.key === itemDetailsKeyEnum.NumberOfPages)!;
      let numberOfPrintingFacesItemDetail: ItemDetailsGetDto = item.details.find(x => x.key === itemDetailsKeyEnum.NumberOfPrintingFaces)!;

      numberOfPagesItemDetail.value = numberOfPages.toString();
      numberOfPagesItemDetail.objectState = ObjectStateEnum.added;

      numberOfPrintingFacesItemDetail.value = numberOfPrintingFaces.toString();
      numberOfPrintingFacesItemDetail.objectState = ObjectStateEnum.added;
    }


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

  public getItem_copy(orderGroupId: number, itemId: number): ItemGetDto {
    return this.deepCopy(this.getItem(orderGroupId, itemId));
  }

  private getItem(orderGroupId: number, itemId: number): ItemGetDto {
    let item = this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.items.find(x => x.id === itemId)!;

    return item;

  }

  public deleteItem(groupId: number, itemId: number): void {
    const item = this.getItem(groupId, itemId);

    if (item.objectState == ObjectStateEnum.temp) {
      let groupItems = this.getOrderGroup(groupId).items;

      const index = groupItems.findIndex(x => x.id === itemId);
      if (index !== -1) {
        groupItems.splice(index, 1);
      }
    } else {
      item.objectState = ObjectStateEnum.deleted;
    }
  }

  // May Be used in items back button page
  // public deleteNewlyAddedItem(groupId: number, itemId: number) {
  //   let groupItems = this.getOrderGroup(groupId).items;

  //   const index = groupItems.findIndex(x => x.id === itemId);
  //   if (index !== -1) {
  //     groupItems.splice(index, 1);
  //   }
  // }


  //=======================
  //#endregion item methods
  //=======================











  //=======================
  //#region GroupServices
  //=======================

  public getOrderGroupServices_copy(orderGroupId: number): OrderGroupServiceGetDto[] {
    let orderGroupServices = this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.orderGroupServices;
    return this.deepCopy(orderGroupServices);
  }

  public getOrderGroupItems_copy(orderGroupId: number): ItemGetDto[] {
    let items = this.orderObject.orderGroups.find(x => x.id === orderGroupId)!.items;
    return this.deepCopy(items);
  }

  public getAllOrderGroupsServices_copy(): OrderGroupServiceGetDto[] {
    let orderGroupsServices = this.orderObject.orderGroups.flatMap(x => x.orderGroupServices);
    return this.deepCopy(orderGroupsServices);
  }

  public addOrderGroupService(orderGroupId: number, service: ServiceGetDto): void {
    let orderGroup: OrderGroupGetDto | undefined = this.orderObject.orderGroups.find(x => x.id === orderGroupId);

    if (orderGroup === undefined) {
      throw new Error('Order group not found');
    }

    if (orderGroup.orderGroupServices.find(x => x.serviceId == service.id)) {
      return;
    }

    let tempId = this.generateTempId(orderGroup.orderGroupServices.map(x => x.id));

    let orderGroupService: OrderGroupServiceGetDto = {
      id: tempId,
      orderGroupId: orderGroupId,
      serviceId: service.id,
      serviceName: service.name,
      objectState: ObjectStateEnum.added
    };

    orderGroup.orderGroupServices.push(orderGroupService);

    this.updateGroupFlagsOnServicesCategories(orderGroup);
  }


  public deleteGroupService(groupId: number, serviceId: number) {
    let orderGroup = this.orderObject.orderGroups.find(x => x.id == groupId);

    if (!orderGroup) {
      throw 'Cannot Find a group with ID = ' + groupId + 'in Shared Data';
    }

    let groupServices = orderGroup.orderGroupServices;


    let groupService = groupServices.find(s => s.serviceId == serviceId);

    if (!groupService) {
      throw 'Cannot Find a Services with ID = ' + serviceId + 'For Group:' + groupId + 'in Shared Data';
    }

    if (groupService.objectState === ObjectStateEnum.temp) {
      let index = groupServices.findIndex(s => s.serviceId == serviceId);
      groupServices.splice(index, 1);
    }
    else {
      groupService.objectState = ObjectStateEnum.deleted;
    }

    this.updateGroupFlagsOnServicesCategories(orderGroup);
  }

  //=======================
  //#endregion Group Services
  //=======================







  //=======================
  //#region Order Servies
  //=======================

  private getOrderServices(): OrderServicesGetDTO[] {
    return this.orderObject.orderServices;
  }

  public getOrderServices_copy(): OrderServicesGetDTO[] {
    return this.deepCopy(this.getOrderServices());
  }

  public setOrderServices(orderServices: OrderServicesGetDTO[]) {
    return this.orderObject.orderServices = orderServices;
  }

  public addOrderServicesDistinct(orderService: OrderServicesGetDTO) {
    if (!this.orderObject.orderServices.find(x => x.serviceId == orderService.serviceId)) {
      this.orderObject.orderServices.push(orderService);
    }
  }


  //=======================
  //#endregion Order Services
  //=======================





  private generateTempId(ids: number[]): number {
    if (ids.length === 0) {
      return 1;
    }

    return Math.max(...ids) + 1;
  }

  private deepCopy<T>(obj: T): T {
    return JSON.parse(JSON.stringify(obj)) as T;
  }
}
