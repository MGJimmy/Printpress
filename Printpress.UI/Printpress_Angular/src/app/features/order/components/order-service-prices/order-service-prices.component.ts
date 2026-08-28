import { Component, Inject, OnInit } from '@angular/core';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { AlertService } from '../../../../core/services/alert.service';
import { ServiceService } from '../../../setup/services/service.service';
import { OrderServicesGetDTO } from '../../models/order-service/order-service-getDto';
import { ServiceCategoryEnum } from '../../../setup/models/service-category.enum';
import { ObjectStateEnum } from '../../../../core/models/object-state.enum';

@Component({
  selector: 'app-order-service-prices',
  standalone: true,
  imports: [FormsModule, MatButtonModule, MatIconModule, CommonModule, MatDialogModule],
  templateUrl: './order-service-prices.component.html',
  styleUrl: './order-service-prices.component.css'
})
export class OrderServicePricesComponent implements OnInit {

  private _orderSharedService: OrderSharedDataService;
  protected isEditMode: boolean;
  protected isZeroOrder: boolean;
  private existingServices: OrderServicesGetDTO[];

  protected _tempServicesList:
    {
      id: string,
      serviceId: string,
      name: string,
      price: number,
      objectState: ObjectStateEnum,
      isNew: boolean,
      isDeleted: boolean
    }[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { orderSharedService: OrderSharedDataService },
    private alertService: AlertService,
    private servicesService: ServiceService,
    private dialogRef: MatDialogRef<OrderServicePricesComponent>
  ) {
    this._orderSharedService = data.orderSharedService;

    let orderState = this._orderSharedService.getOrderObject_copy().objectState;
    this.isEditMode = orderState != ObjectStateEnum.temp && orderState != ObjectStateEnum.added;
    this.isZeroOrder = this._orderSharedService.getOrderObject_copy().isZeroOrder === true;
    this.existingServices = this._orderSharedService.getOrderServices_copy(true);
  }

  async ngOnInit() {
    if (this.isEditMode) {
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

      if (service.serviceCategoryCode === ServiceCategoryEnum.Selling) {
        continue;
      }
      const groupService = allOrderGroupServices.find(x => x.serviceId == serviceId);

      this._tempServicesList.push({
        id: orderService.id,
        serviceId: service.id,
        name: groupService?.isCover ? `غلاف ${service.name}` : service.name,
        price: orderService.price,
        objectState: orderService.objectState,
        isNew: orderService.objectState == ObjectStateEnum.temp || orderService.objectState == ObjectStateEnum.added,
        isDeleted: (!groupService || groupService.objectState == ObjectStateEnum.deleted)
      });
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

      if (service.serviceCategoryCode === ServiceCategoryEnum.Selling) {
        continue;
      }

      const groupService = allOrderGroupServices[i];
      this._tempServicesList.push({
        id: this._orderSharedService.generateEmptyId(),
        serviceId: service.id,
        name: groupService?.isCover ? `غلاف ${service.name}` : service.name,
        price: service.price,
        objectState: ObjectStateEnum.temp,
        isNew: true,
        isDeleted: false
      });
    }
  }

  protected onPriceChange(): void {
    if (this.isZeroOrder && this._tempServicesList.some(x => !x.isDeleted && (x.price ?? 0) > 0)) {
      this.isZeroOrder = false;
    }
  }

  protected makeZeroOrder(): void {
    for (const service of this._tempServicesList) {
      if (!service.isDeleted) {
        service.price = 0;
      }
    }
    this.isZeroOrder = true;
    this.save(true);
  }

  protected save_Click() {
    this.save(false);
  }

  private save(asZeroOrder: boolean): void {
    if (!asZeroOrder && !this.validateOrderPrices()) {
      return;
    }

    const isZeroOrder = asZeroOrder || this.isZeroOrder;
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
        price: isZeroOrder ? 0 : x.price,
        objectState: objectState
      };
    });

    this.dialogRef.close({ services: orderServices, isZeroOrder });
  }

  private validateOrderPrices(): boolean {
    const active = this._tempServicesList.filter(x => !x.isDeleted);
    if (active.some(x => x.price === null || x.price === undefined || Number.isNaN(Number(x.price)) || Number(x.price) < 0)) {
      this.alertService.showError('يجب تحديد سعر لكل الخدمات، ويسمح بصفر لخدمة واحدة أو أكثر');
      return false;
    }

    return true;
  }
}
