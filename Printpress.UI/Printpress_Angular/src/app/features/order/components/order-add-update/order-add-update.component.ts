import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { imports } from './order-add-update.imports';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-add-update',
  standalone: true,
  imports: imports,
  templateUrl: './order-add-update.component.html',
  styleUrl: './order-add-update.component.css',
})
export class OrderAddUpdateComponent implements OnInit {

  public componentMode: ComponentMode;
  displayedColumns = ['no', 'name', 'deliveryDate', 'action'];
  dataSource = new MatTableDataSource<OrderGroupGridViewModel>(ELEMENT_DATA);

  constructor(private router: Router,
    private OrderSharedService: OrderSharedDataService,
    private orderService: OrderService
  ) {
    this.componentMode = new ComponentMode(router);
  }

  ngOnInit(): void {

  }

  saveOrder(){
    const orderDTO = this.OrderSharedService.getOrderObject()
    console.log(orderDTO);
    this.orderService.insertOrder(orderDTO).subscribe();
  }
}

export interface OrderGroupGridViewModel {
  no: number;
  name: string;
  deliveryDate: number;
}
const ELEMENT_DATA: OrderGroupGridViewModel[] = [
  { no: 1, name: 'Hydrogen', deliveryDate: 1.0079 },
  { no: 2, name: 'Helium', deliveryDate: 4.0026 },
  { no: 3, name: 'Lithium', deliveryDate: 6.941 },
];

export class ComponentMode {
  public isEditMode: boolean;
  public isViewMode: boolean;
  public isaddMode: boolean;
  constructor(private router: Router) {
    this.isViewMode = this.router.url.includes('view');
    this.isEditMode = this.router.url.includes('edit');
    this.isaddMode = this.router.url.includes('add');
  }
}
