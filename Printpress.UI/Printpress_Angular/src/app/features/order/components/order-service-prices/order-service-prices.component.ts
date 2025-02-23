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
  private _tempServicesList!: any;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { orderSharedService: OrderSharedDataService },
    private orderService: OrderService,
    private router: Router,
  ) {
    this._orderSharedService = data.orderSharedService;
    // this._tempServicesList = this._orderSharedService.getOrderGroupServices(1); //getOrderServices()

  }

  ngOnInit() {

  }

  laserPrice: number = 1.0;
  colorPrice: number = 1.0;

  editPrice(type: string) {
    if (type === 'laser') {
      alert(`Editing laser paper price: ${this.laserPrice}`);
    } else if (type === 'color') {
      alert(`Editing color paper price: ${this.colorPrice}`);
    }
  }

  save_Click() {
    //this._orderSharedService.setOrderServices(this._tempServicesList);

    const orderDTO = this._orderSharedService.getOrderObject()
    console.log(orderDTO);
    this.orderService.insertOrder(orderDTO).subscribe();

    this.navigateToOrderListPage();
  }

  private navigateToOrderListPage() {
    this.router.navigate([this._orderSharedService.getOrderPageRoute()]);
  }
}
