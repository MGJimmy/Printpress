import { Component, Inject } from '@angular/core';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { OrderGroupServiceGetDto } from '../../models/orderGroupService/order-group-service-get.Dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-order-service-prices',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule,
    CommonModule, MatDialogModule],
  templateUrl: './order-service-prices.component.html',
  styleUrl: './order-service-prices.component.css'
})
export class OrderServicePricesComponent {

  private _orderSharedService!: OrderSharedDataService;
  protected _tempServicesList!: any[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { orderSharedService: OrderSharedDataService },
    private orderService: OrderService,
    private router: Router,
    private alertService: AlertService
  ) {

    this._orderSharedService = data.orderSharedService;
    // this._tempServicesList = this._orderSharedService.getAllOrderGroupsServices();

    this._tempServicesList = [
      { id: 1, price: 5, serviceName: 'test' },
      { id: 2, price: 10, serviceName: 'test' },
      { id: 3, price: 15, serviceName: 'test' }
    ];
  }

  save_Click() {

    // Validate
    if (!this.validateOrderPrices()) {
      return;
    }
    // this._orderSharedService.setOrderServices(this._tempServicesList);

    const orderDTO = this._orderSharedService.getOrderObject()
    console.log(orderDTO);
    // Map to order upsert
    this.orderService.insertOrder(orderDTO).subscribe(
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
