import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { imports } from './order-add-update.imports';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderService } from '../../services/order.service';
import { ClientService } from '../../../client/services/client.service';
import { ClientGetDto } from '../../../client/models/client-get.dto';
import { ComponentMode } from '../../../../shared/models/ComponentMode';
import { MatDialog } from '@angular/material/dialog';
import { AddClientComponent } from '../../../client/components/add-client/add-client.component';
import { OrderServicePricesComponent } from '../order-service-prices/order-service-prices.component';
import { AlertService } from '../../../../core/services/alert.service';
import { OrderGetDto } from '../../models/order/order-get.Dto';
import { firstValueFrom } from 'rxjs';
import { OrderGroupGetDto } from '../../models/orderGroup/order-group-get.Dto';

@Component({
  selector: 'app-order-add-update',
  standalone: true,
  imports: imports,
  templateUrl: './order-add-update.component.html',
  styleUrl: './order-add-update.component.css',
})
export class OrderAddUpdateComponent implements OnInit {

  public componentMode: ComponentMode;
  public displayedColumns = ['name', 'deliveryDate', 'action'];
  public orderGroupGridDataSource !: MatTableDataSource<OrderGroupGridViewModel>;
  public clients: ClientGetDto[] = [];
  public selectedClientId!: number
  public orderName!: string;
  public orderGetDto!:OrderGetDto;

  constructor(private router: Router,
    private OrderSharedService: OrderSharedDataService,
    private clientService: ClientService,
    private dialog: MatDialog,
    private alertService: AlertService,
    private activedRoute: ActivatedRoute,
    private orderService: OrderService
  ) {
    this.componentMode = new ComponentMode(this.router);
  }

 async ngOnInit() {
    if (this.componentMode.isViewMode || this.componentMode.isEditMode) {
      if(this.componentMode.isViewMode){
        this.displayedColumns = this.displayedColumns.filter(col => col !== 'action')
      }
      let orderId = Number(this.activedRoute.snapshot.paramMap.get('id'));         
      let response = await firstValueFrom(this.orderService.getOrderById(orderId));
      this.orderGetDto = response.data
      this.OrderSharedService.setOrderObject(this.orderGetDto);
    }

    else{ // case add mode
     this.orderGetDto = this.OrderSharedService.getOrderObject();       
    }

    this.orderGroupGridDataSource = new MatTableDataSource<OrderGroupGridViewModel>(this.MapToOrderGroupGridViewModel(this.orderGetDto.orderGroups));

    let response = await firstValueFrom(this.clientService.getAll())
    this.clients = response.data;
  }

 public saveOrder_Click() {

    if (!this.validateOrderData()) {
      return;
    }

    this.openServicePricesDialog();
  }

 private validateOrderData(): boolean {
    const emptyGroupsList = this.OrderSharedService.getOrderObject().orderGroups.length == 0;
    if (emptyGroupsList) {
      this.alertService.showError('يجب إضافة مجموعات للطلبية');
      return false;
    }

    return true;
  }

 private openServicePricesDialog() {
    this.dialog.open(OrderServicePricesComponent, {
      data: { orderSharedService: this.OrderSharedService },
      height: '550px',
      width: '1000px'
    });
  }

 public openDialog() {
    this.dialog.open(AddClientComponent, {
      width: '600px',
     
    });
  }
  
 private MapToOrderGroupGridViewModel( orderGroupGetDtos:OrderGroupGetDto[] ):OrderGroupGridViewModel[]{
  if(!orderGroupGetDtos){
    return [];
  }

  return orderGroupGetDtos.map((orderGroup, index) => {
      return {
        name: orderGroup.name,
        deliveryDate: orderGroup.deliveryDate
    }});
  }
}

 interface OrderGroupGridViewModel {
  name: string;
  deliveryDate?: Date;
}



