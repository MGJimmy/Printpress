import { Component, Inject, OnInit } from '@angular/core';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { AlertService } from '../../../../core/services/alert.service';
import { ServiceService } from '../../../setup/services/service.service';
import { OrderServicesGetDTO } from '../../models/order-service/order-service-getDto';
import { ServiceCategoryEnum } from '../../../setup/models/service-category.enum';
import { ObjectStateEnum } from '../../../../core/models/object-state.enum';
import { OrderRoutingService } from '../../services/order-routing.service';

@Component({
  selector: 'app-order-service-prices',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule,
    CommonModule, MatDialogModule],
  templateUrl: './order-service-prices.component.html',
  styleUrl: './order-service-prices.component.css'
})
export class OrderServicePricesComponent implements OnInit {

  private _orderSharedService: OrderSharedDataService;
  protected isEditMode: boolean;
  private existingServices: OrderServicesGetDTO[];

  protected _tempServicesList:
    {
      id: number,
      serviceId: number,
      name: string,
      price: number,
      objectState: ObjectStateEnum,
      isNew: boolean,
      isDeleted: boolean
    }[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { orderSharedService: OrderSharedDataService },
    private router: Router,
    private alertService: AlertService,
    private servicesService: ServiceService,
    private orderRoutingService: OrderRoutingService,
    private dialogRef: MatDialogRef<OrderServicePricesComponent>
  ) {
    this._orderSharedService = data.orderSharedService;

    let orderState = this._orderSharedService.getOrderObject_copy().objectState;
    this.isEditMode = orderState != ObjectStateEnum.temp && orderState != ObjectStateEnum.added;

    this.existingServices = this._orderSharedService.getOrderServices_copy(true);
  }

  async ngOnInit() {
    if(this.isEditMode){
      await this.fillFromExistingServices();
    }
    await this.fillFromNewGroupServices();
  }

  private async fillFromExistingServices() {
    let allOrderGroupServices = this._orderSharedService.getAllOrderGroupsServices_copy();

    for (let i = 0; i < this.existingServices.length; i++) {
      const orderService = this.existingServices[i];
      const serviceId = orderService.serviceId;
      
      if (this._tempServicesList.find(x => x.serviceId == serviceId)) {
        continue;
      }
      
      const service = await this.servicesService.getServiceById(serviceId);
      
      // Selling  services should not be edited in this page
      if (service.serviceCategory == ServiceCategoryEnum.Selling) {
        continue;
      }
      const groupService = allOrderGroupServices.find(x => x.serviceId == serviceId);
      
      const tempService: {
        id: number, serviceId: number, name: string, price: number, objectState: ObjectStateEnum, isNew: boolean,
        isDeleted: boolean
      } = {
        id: orderService.id,
        serviceId: service.id,
        name: service.name,
        price: orderService.price,
        objectState: orderService.objectState,
        isNew: orderService.objectState == ObjectStateEnum.temp || orderService.objectState == ObjectStateEnum.added,
        isDeleted: (!groupService || groupService.objectState == ObjectStateEnum.deleted)
      };

      this._tempServicesList.push(tempService);
    }
  }

  private async fillFromNewGroupServices() {
    let allOrderGroupServices = this._orderSharedService.getAllOrderGroupsServices_copy();

    for (let i = 0; i < allOrderGroupServices.length; i++) {
      const serviceId = allOrderGroupServices[i].serviceId;

      if (
        this._tempServicesList.find(x => x.serviceId == serviceId) ||
        this.existingServices.find(x => x.serviceId == serviceId)) {
        continue;
      }

      const service = await this.servicesService.getServiceById(serviceId);

      // Selling  services should not be edited in this page
      if (service.serviceCategory == ServiceCategoryEnum.Selling) {
        continue;
      }

      const tempService: {
        id: number, serviceId: number, name: string, price: number,
        objectState: ObjectStateEnum, isNew: boolean, isDeleted: boolean
      } = {
        id: 0,
        serviceId: service.id,
        name: service.name,
        price: service.price,
        objectState: ObjectStateEnum.temp,
        isNew: true,
        isDeleted: false
      };

      this._tempServicesList.push(tempService);
    }
  }

  protected save_Click() {
    if (!this.validateOrderPrices()) {
      return;
    }

    const orderServices: OrderServicesGetDTO[] = this._tempServicesList.map(x => {
      let objectState: ObjectStateEnum;
      if (x.isNew) {
        objectState = ObjectStateEnum.added;
      } else if (x.isDeleted) {
        objectState = ObjectStateEnum.deleted;
      } else if (x.price != this.existingServices.find(s => s.id == x.id)?.price) {
        objectState = ObjectStateEnum.modified;
      } else {
        objectState = ObjectStateEnum.unchanged;
      }

      return {
        id: x.id,
        serviceId: x.serviceId,
        price: x.price,
        objectState: objectState
      }
    });

    // Return the updated services to the parent component
    this.dialogRef.close(orderServices);
  }

  private validateOrderPrices(): boolean {
    if (this.isAnyServicePriceEmpty()) {
      this.alertService.showError('يجب تحديد سعر لكل الخدمات');
      return false;
    }

    return true;
  }

  private isAnyServicePriceEmpty() {
    return this._tempServicesList.some(x => !x.price);
  }

  private navigateToOrderListPage() {
    this.router.navigate([this.orderRoutingService.getOrderListRoute()]);
  }
}
