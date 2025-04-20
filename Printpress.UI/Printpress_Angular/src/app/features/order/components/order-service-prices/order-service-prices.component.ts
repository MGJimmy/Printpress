import { Component, Inject, OnInit } from '@angular/core';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { AlertService } from '../../../../core/services/alert.service';
import { mapOrderGetToUpsert } from '../../models/order-mapper';
import { ServiceService } from '../../../setup/services/service.service';
import { OrderServicesGetDTO } from '../../models/order-service/order-service-getDto';
import { ServiceCategoryEnum } from '../../../setup/models/service-category.enum';
import { ObjectStateEnum } from '../../../../core/models/object-state.enum';

@Component({
  selector: 'app-order-service-prices',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule,
    CommonModule, MatDialogModule],
  templateUrl: './order-service-prices.component.html',
  styleUrl: './order-service-prices.component.css'
})
export class OrderServicePricesComponent implements OnInit {

  private _orderSharedService!: OrderSharedDataService;
  protected _tempServicesList!: { serviceId: number, name: string, price: number, objectState: ObjectStateEnum, isNew: boolean }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { orderSharedService: OrderSharedDataService },
    private orderService: OrderService,
    private router: Router,
    private alertService: AlertService,
    private servicesService: ServiceService
  ) {
    this._orderSharedService = data.orderSharedService;
  }

  async ngOnInit() {
    this._tempServicesList = [];
    let allOrderGroupServices = this._orderSharedService.getAllOrderGroupsServices_copy();
    
    for (let i = 0; i < allOrderGroupServices.length; i++) {
      const currentService = allOrderGroupServices[i];
      const serviceId = currentService.serviceId;

      if (this._tempServicesList.find(x => x.serviceId == serviceId)) {
        continue;
      }

      const service = await this.servicesService.getServiceById(serviceId);

      // Selling  services should not be edited in this page
      if (service.serviceCategory == ServiceCategoryEnum.Selling) {
        continue;
      }

      const orderObjectState = this._orderSharedService.getOrderObject_copy().objectState;

      
      const tempService: { serviceId: number, name: string, price: number, objectState: ObjectStateEnum, isNew: boolean } = {
        serviceId: service.id,
        name: service.name,
        price: service.price,
        objectState: currentService.objectState,
        isNew: orderObjectState != ObjectStateEnum.added && orderObjectState != ObjectStateEnum.temp && currentService.objectState == ObjectStateEnum.added
      };

      this._tempServicesList.push(tempService);
    }
  }

  protected save_Click() {

    // Validate
    if (!this.validateOrderPrices()) {
      return;
    }

    const orderServices: OrderServicesGetDTO[] = this._tempServicesList.map(x => {
      return {
        id: 0, //////////////////////////// ???
        serviceId: x.serviceId,
        price: x.price,
        objectState: x.objectState
      }
    })

    this._orderSharedService.setOrderServices(orderServices);

    this._orderSharedService.updateOrderObjectState();
    
    const orderDTO = this._orderSharedService.getOrderObject_copy()

    const orderUpsertDTO = mapOrderGetToUpsert(orderDTO);

    let upsertObservable = orderDTO.objectState == ObjectStateEnum.added || orderDTO.objectState == ObjectStateEnum.temp ?
                     this.orderService.insertOrder(orderUpsertDTO) :
                     this.orderService.updateOrder(orderUpsertDTO);

    upsertObservable.subscribe(
      (response) => {
        this.alertService.showSuccess('تم حفظ الطلبية بنجاح');
        this.navigateToOrderListPage();
      }, (error) => {
        this.alertService.showError('حدث خطأ أثناء حفظ الطلبية');
      }
    );
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
    this.router.navigate([this._orderSharedService.getOrderListRoute()]);
  }
}
