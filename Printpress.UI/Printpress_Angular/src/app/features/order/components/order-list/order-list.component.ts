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
import { MatDialog } from '@angular/material/dialog';
import { OrderStatus } from '../../models/enums/order-status.enum';
import { isStatus, normalizeStatus, statusBadgeClass, statusI18nKey } from '../../models/enums/status-display';
import { UserRoleEnum } from '../../../../core/models/user-role.enum';
import { TranslationService } from '../../../../core/services/translation.service';
import { InvoiceGroupSelectDialogComponent } from '../invoice-group-select-dialog/invoice-group-select-dialog.component';

@Component({
  selector: 'app-order-view',
  standalone: true,
  imports: imports,
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css'
})
export class OrderListComponent implements OnInit {
  public OrderStatus = OrderStatus; // Make enum available in template
  public dataSource : MatTableDataSource<OrderSummaryDto>;
  public totalCount:number ;
  public isEditMode:boolean ;
  public displayedColumns : string[];
  protected userRoleEnum = UserRoleEnum;

  constructor(
    private orderService: OrderService,
    private alertService: AlertService,
    private dialogService: DialogService,
    private router: Router,
    private orderRoutingService: OrderRoutingService,
    private dialog: MatDialog,
    private _t: TranslationService
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

  protected canDeleteOrder(orderStatus: string): boolean {
    return !isStatus(orderStatus, OrderStatus.Delivered, OrderStatus.Completed);
  }

  public async onDeleteOrder(id: string) {
    const dialogData = {
      title: this._t.t('orders.confirm_delete'),
      message: this._t.t('orders.delete_order_msg'),
      confirmText: this._t.t('shared.yes'),
      cancelText: this._t.t('shared.cancel'),
    };

    const confirmed = await firstValueFrom(this.dialogService.confirmDialog(dialogData));

    if (confirmed) {
      try {
        await firstValueFrom(this.orderService.deleteOrder(id));
        this.alertService.showSuccess(this._t.t('orders.order_deleted'));
        await this.loadOrders();
      } catch (error) {
        this.alertService.showError(this._t.t('orders.error_deleting_order'));
      }
    }
  }

  protected onViewOrder(id: string) {
    this.router.navigate([this.orderRoutingService.getOrderViewRoute(id)]);
  }

  protected onEditOrder(id: string) {
    this.router.navigate([this.orderRoutingService.getOrderEditRoute(id)]);
  }

  protected onAddOrder() {
    this.router.navigate([this.orderRoutingService.getOrderAddRoute()]);
  }

  protected async generateInvoice_Click(orderId: string) {
    const selectedIds = await firstValueFrom(
      this.dialog.open(InvoiceGroupSelectDialogComponent, {
        width: '420px',
        data: { orderId },
        disableClose: true
      }).afterClosed()
    ) as string[] | undefined;

    if (!selectedIds?.length) {
      return;
    }

    window.open(`report-viewer?reportName=invoice&id=${orderId}&groupIds=${selectedIds.join(',')}`, '_blank');
  }

  getStatusBadgeClass(status: OrderStatus | string | number): string {
    return statusBadgeClass(status);
  }

  getStatusText(status: OrderStatus | string | number): string {
    return this._t.t(statusI18nKey(status));
  }

  protected isStatus = isStatus;
}

