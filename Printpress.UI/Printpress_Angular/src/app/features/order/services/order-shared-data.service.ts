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
import { OrderMainDataDto } from '../models/order/order-main-data.Dto';
import { Subject } from 'rxjs';
import { IObjectState } from '../../../core/models/i-object-state';

/*
  Notes: 
    * Return new object when returning object from the service to prevent changing the original object. using spread operator. {...object}
*/

@Injectable()
export class OrderSharedDataService {

  private orderObject!: OrderGetDto;

  constructor(
    private serviceService: ServiceService
  ) {
    this.initializeOrderObject();
  }

  private filterDeletedObject<T extends IObjectState>(obj: T): T {
    return this.filterDeletedObjectArray([obj])[0];
  }

  private filterDeletedObjectArray<T extends IObjectState>(objs: T[]): T[] {
    return objs
      .filter(obj => obj.objectState !== ObjectStateEnum.deleted)
      .map(obj => {
        // For each property of the object
        for (const key of Object.keys(obj)) {
          const value = (obj as any)[key];

          // If the property is an array of objects that implement IObjectState
          if (Array.isArray(value) && value.every(this.isObjectState)) {
            (obj as any)[key] = this.filterDeletedObjectArray(value);
          }
        }

        return obj;
      });
  }

  // Helper to check if an object implements IObjectState
  private isObjectState(obj: any): obj is IObjectState {
    return obj && typeof obj === 'object' && 'objectState' in obj;
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
      status:'',
      objectState: ObjectStateEnum.temp,
      orderGroups: [],
      orderServices: []
    };
  }



  public setOrderObject(order: OrderGetDto): void {
    this.orderObject = order;

    // set the flags because it used in group and item components
    this.orderObject.orderGroups.forEach(group => {
      this.updateGroupFlagsOnServicesCategories(group.id);
    });
  }

  public setOrderName(name: string): void {
    this.orderObject.name = name;
  }
  public setOrderClient(clientId: number): void {
    this.orderObject.clientId = clientId;
  }

  public updateOrderObjectState(): void {
    this.orderObject.objectState = this.orderObject.objectState === ObjectStateEnum.temp ? ObjectStateEnum.added : ObjectStateEnum.modified;
  }

  public getOrderObject_copy(includeDeletedObjects: boolean = false): OrderGetDto {
    let order = this.deepCopy(this.orderObject);
    return includeDeletedObjects ? order : this.filterDeletedObject(order);
  }

  public refreshOrderMainData(orderMainData: OrderMainDataDto) {
    this.orderObject.totalPrice = orderMainData.totalPrice ?? 0;
    this.orderObject.totalPaid = orderMainData.totalPaid ?? 0;
    this.orderObject.clientName = orderMainData.clientName;
    this.orderObject.name = orderMainData.name;
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
    let orderGroup = this.deepCopy(this.getOrderGroup(id));
    return this.filterDeletedObject(orderGroup);
  }

  private getOrderGroup(id: number): OrderGroupGetDto {
    let group = this.orderObject.orderGroups.filter(x => x.objectState !== ObjectStateEnum.deleted)
      .find(x => x.id == id);

    if (!group) {
      throw 'cannot find a group with id = ' + id;
    }

    return group;
  }

  private getOrderGroups(): OrderGroupGetDto[] {
    return this.orderObject.orderGroups.filter(x => x.objectState !== ObjectStateEnum.deleted);
  }

  public getOrderGroups_Copy(): OrderGroupGetDto[] {
    let orderGroups = this.deepCopy(this.getOrderGroups());
    return this.filterDeletedObjectArray(orderGroups);
  }

  public updateOrderGroupName(id: number, name: string) {
    let orderGroup = this.getOrderGroup(id);
    orderGroup.name = name;
  }

  public updateOrderGroup(id: number) {
    let orderGroup = this.getOrderGroup(id);
    orderGroup.objectState = this.getObjectState(orderGroup.objectState, ObjectStateEnum.modified);
  }



  public addOrderGroup(id: number) {
    let orderGroup = this.getOrderGroup(id);
    orderGroup.objectState = ObjectStateEnum.added;
  }

  public deleteGroup(groupId: number) {
    let group = this.getOrderGroup(groupId);

    if (group.objectState == ObjectStateEnum.temp || group.objectState == ObjectStateEnum.added) {
      this.hardDeleteGroup(groupId);
    } else {
      group.objectState = ObjectStateEnum.deleted;
    }
  }
  
  private hardDeleteGroup(groupId: number) {
    const index = this.orderObject.orderGroups.findIndex(x => x.id === groupId);
    if (index !== -1) {
      this.orderObject.orderGroups.splice(index, 1);
    }
  }


  private updateGroupFlagsOnServicesCategories(groupId: number) {
    const group = this.getOrderGroup(groupId);
    let groupServices = group.orderGroupServices.filter(s => s.objectState !== ObjectStateEnum.deleted)
      .map(x => x.serviceId);

    this.serviceService.getServices(groupServices).subscribe(services => {

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
    let orderGroup: OrderGroupGetDto | undefined = this.getOrderGroup(orderGroupId);

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

  public addUpdateItem(isUpdate:boolean, orderGroupId: number, itemId: number, name: string, quantity: number, price: number, numberOfPages: number | undefined, numberOfPrintingFaces: number | undefined): void {
    let newObjectState: ObjectStateEnum = isUpdate ? ObjectStateEnum.modified : ObjectStateEnum.added;

    let orderGroup = this.getOrderGroup(orderGroupId);

    let item: ItemGetDto = this.getItem(orderGroupId, itemId)!;

    item.name = name;
    item.quantity = quantity;
    item.price = price;
    item.objectState = this.getObjectState(item.objectState, newObjectState);

    if (orderGroup.isHasPrintingService) {
      if (!numberOfPages || !numberOfPrintingFaces) {
        throw new Error('Number of pages and number of printing faces are required for printing service');
      }

      let numberOfPagesItemDetail: ItemDetailsGetDto = item.details.find(x => x.key === itemDetailsKeyEnum.NumberOfPages)!;
      let numberOfPrintingFacesItemDetail: ItemDetailsGetDto = item.details.find(x => x.key === itemDetailsKeyEnum.NumberOfPrintingFaces)!;

      numberOfPagesItemDetail.value = numberOfPages.toString();
      numberOfPagesItemDetail.objectState = this.getObjectState(numberOfPagesItemDetail.objectState, newObjectState);

      numberOfPrintingFacesItemDetail.value = numberOfPrintingFaces.toString();
      numberOfPrintingFacesItemDetail.objectState = this.getObjectState(numberOfPrintingFacesItemDetail.objectState, newObjectState);
    }
  }

  public getItem_copy(orderGroupId: number, itemId: number): ItemGetDto {
    let item = this.deepCopy(this.getItem(orderGroupId, itemId));
    return this.filterDeletedObject(item);
  }

  private getItem(orderGroupId: number, itemId: number): ItemGetDto {
    let group = this.getOrderGroup(orderGroupId);

    let item = group!.items.filter(x => x.objectState !== ObjectStateEnum.deleted)
      .find(x => x.id === itemId)!;

    if (item === undefined) {
      throw new Error('item not found');
    }
    return item;

  }

  public getOrderGroupItems_copy(orderGroupId: number): ItemGetDto[] {
    let items = this.getOrderGroupItems(orderGroupId);
    items = this.deepCopy(items);
    return this.filterDeletedObjectArray(items);
  }

  private getOrderGroupItems(orderGroupId: number): ItemGetDto[] {
    let orderGroup = this.getOrderGroup(orderGroupId);
    return orderGroup!.items.filter(x => x.objectState !== ObjectStateEnum.deleted);
  }

  public deleteItem(groupId: number, itemId: number): void {
    const item = this.getItem(groupId, itemId);

    if (item.objectState == ObjectStateEnum.temp || item.objectState == ObjectStateEnum.added) {
      this.hardDeleteItem(groupId, itemId);
    } else {
      item.objectState = ObjectStateEnum.deleted;
    }
  }

  private hardDeleteItem(groupId: number, itemId: number): void {
    const group = this.getOrderGroup(groupId);
    const index = group.items.findIndex(x => x.id === itemId);
    if (index !== -1) {
      group.items.splice(index, 1);
    }
  }


  //=======================
  //#endregion item methods
  //=======================











  //=======================
  //#region GroupServices
  //=======================

  public getOrderGroupServices_copy(orderGroupId: number): OrderGroupServiceGetDto[] {
    let orderGroupServices = this.getOrderGroupServices(orderGroupId);

    let groupServices = this.deepCopy(orderGroupServices);
    return this.filterDeletedObjectArray(groupServices);
  }


  public getOrderGroupServices(orderGroupId: number): OrderGroupServiceGetDto[] {
    let group = this.getOrderGroup(orderGroupId);

    return group!.orderGroupServices.filter(x => x.objectState !== ObjectStateEnum.deleted);
  }


  public getAllOrderGroupsServices_copy(): OrderGroupServiceGetDto[] {
    let orderGroupsServices = this.getOrderGroups().flatMap(x => x.orderGroupServices);
    orderGroupsServices = this.deepCopy(orderGroupsServices);
    return this.filterDeletedObjectArray(orderGroupsServices);
  }

  private getOrderGroupService(orderGroupId: number, serviceId: number): OrderGroupServiceGetDto {
    let orderGroup = this.getOrderGroup(orderGroupId);

    let orderGroupService = orderGroup.orderGroupServices.filter(x => x.objectState !== ObjectStateEnum.deleted)
      .find(x => x.serviceId === serviceId);

    if (orderGroupService === undefined) {
      throw new Error('Order group service not found');
    }

    return orderGroupService;
  }

  public addOrderGroupService(orderGroupId: number, service: ServiceGetDto): void {
    let orderGroup: OrderGroupGetDto = this.getOrderGroup(orderGroupId);

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

    this.updateGroupFlagsOnServicesCategories(orderGroupId);
  }


  public deleteGroupService(groupId: number, serviceId: number) {

    let groupService = this.getOrderGroupService(groupId, serviceId);

    if (groupService.objectState === ObjectStateEnum.temp || groupService.objectState === ObjectStateEnum.added) {
      this.hardDeleteGroupService(groupId, serviceId);
    }
    else {
      groupService.objectState = ObjectStateEnum.deleted;
    }

    this.updateGroupFlagsOnServicesCategories(groupId);
  }

  private hardDeleteGroupService(groupId: number, serviceId: number) {
    const group = this.getOrderGroup(groupId);
    const index = group.orderGroupServices.findIndex(x => x.serviceId === serviceId);
    if (index !== -1) {
      group.orderGroupServices.splice(index, 1);
    }
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

  public getOrderServices_copy(includeDeleted: boolean = false): OrderServicesGetDTO[] {
    let orderServices = this.deepCopy(this.getOrderServices());
    return includeDeleted ? orderServices : this.filterDeletedObjectArray(orderServices);
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


  private getObjectState(currentState: ObjectStateEnum, newState: ObjectStateEnum): ObjectStateEnum {
    // if modifing a newly added object, it should be added not modified
    if (currentState == ObjectStateEnum.added && newState == ObjectStateEnum.modified) {
      return ObjectStateEnum.added;
    }

    return newState;
  }


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
