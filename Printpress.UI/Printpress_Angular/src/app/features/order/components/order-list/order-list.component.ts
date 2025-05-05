import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material/table';
import { imports } from './order-list.imports';
import { OrderService } from '../../services/order.service';
import { firstValueFrom } from 'rxjs';
import { OrderSummaryDto } from '../../models/order/order-summary.Dto';
import {DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';
import { PageEvent } from '@angular/material/paginator';
import { AlertService } from '../../../../core/services/alert.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { OrderRoutingService } from '../../services/order-routing.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-view',
  standalone: true,
  imports: imports,
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css'
})
export class OrderListComponent implements OnInit {

  public dataSource : MatTableDataSource<OrderSummaryDto>;
  public totalCount:number ;
  public isEditMode:boolean ;
  public displayedColumns : string[];

  constructor(
    private orderService: OrderService,
    private alertService: AlertService,
    private dialogService: DialogService,
    private router: Router,
    private orderRoutingService: OrderRoutingService
  )  {
    this.dataSource = new MatTableDataSource<OrderSummaryDto>();
    this.totalCount = 0;
    this.isEditMode = false;
    this.displayedColumns = ['orderName', 'clientName', 'totalAmount', 'paidAmount','orderStatus' ,'createdAt', 'action' ];
  }

  async ngOnInit(){
    await this.loadOrders();
  }

  private async loadOrders() {
    const response = await firstValueFrom(this.orderService.getOrdersSummaryList(DEFAULT_PAGE_SIZE,DEFAULT_PAGE_NUMBER));
    this.dataSource.data = response.data.items;
    this.totalCount = response.data.totalCount;
  }

  public async onPageChange(event:PageEvent){
    const pageSize = event.pageSize;
    const pageNumber = event.pageIndex + 1;   

    const response = await firstValueFrom(this.orderService.getOrdersSummaryList(pageSize,pageNumber));
    this.dataSource.data = response.data.items;
    this.totalCount = response.data.totalCount;
  }

  public async onDeleteOrder(id: number) {
    const dialogData = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذه الطلبيه؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    const confirmed = await firstValueFrom(this.dialogService.confirmDialog(dialogData));
    
    if (confirmed) {
      try {
        await firstValueFrom(this.orderService.deleteOrder(id));
        this.alertService.showSuccess('تم حذف الطلبيه بنجاح');
        await this.loadOrders();
      } catch (error) {
        this.alertService.showError('حدث خطأ اثناء حذف الطلبيه');
      }
    }
  }

  protected onViewOrder(id: number) {
    this.router.navigate([this.orderRoutingService.getOrderViewRoute(id)]);
  }

  protected onEditOrder(id: number) {
    this.router.navigate([this.orderRoutingService.getOrderEditRoute(id)]);
  }

  protected onAddOrder() {
    this.router.navigate([this.orderRoutingService.getOrderAddRoute()]);
  }

  protected generateInvoice_Click(orderId: number) {
    window.open(`report-viewer?reportName=invoice&id=${orderId}`, "_blank");
  }
}

