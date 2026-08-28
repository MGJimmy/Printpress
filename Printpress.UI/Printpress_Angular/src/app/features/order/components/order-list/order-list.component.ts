import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { provideNativeDateAdapter } from '@angular/material/core';
import { imports } from './order-list.imports';
import { OrderService } from '../../services/order.service';
import { firstValueFrom } from 'rxjs';
import { OrderSummaryDto } from '../../models/order/order-summary.Dto';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';
import { PageEvent } from '@angular/material/paginator';
import { AlertService } from '../../../../core/services/alert.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { OrderRoutingService } from '../../services/order-routing.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { OrderStatus } from '../../models/enums/order-status.enum';
import { isStatus, statusBadgeClass, statusI18nKey } from '../../models/enums/status-display';
import { UserRoleEnum } from '../../../../core/models/user-role.enum';
import { TranslationService } from '../../../../core/services/translation.service';
import { InvoiceGroupSelectDialogComponent } from '../invoice-group-select-dialog/invoice-group-select-dialog.component';
import { ClientService } from '../../../client/services/client.service';
import { SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';

@Component({
  selector: 'app-order-view',
  standalone: true,
  imports: imports,
  providers: [provideNativeDateAdapter()],
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css'
})
export class OrderListComponent implements OnInit {
  public OrderStatus = OrderStatus;
  public dataSource: MatTableDataSource<OrderSummaryDto>;
  public totalCount = 0;
  public displayedColumns = ['orderName', 'clientName', 'totalAmount', 'paidAmount', 'orderStatus', 'createdAt', 'action'];
  protected userRoleEnum = UserRoleEnum;
  protected isLoading = false;
  protected clientItems: SearchSelectItem[] = [];

  protected statusOptions = [
    { value: 1, label: 'جديد' },
    { value: 2, label: 'قيد التنفيذ' },
    { value: 3, label: 'مكتمل' },
    { value: 4, label: 'تم التسليم' }
  ];

  filterForm: FormGroup<{
    search: FormControl<string>;
    clientId: FormControl<string | null>;
    status: FormControl<number | null>;
    isZeroOrder: FormControl<boolean | null>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  private pageNumber = DEFAULT_PAGE_NUMBER;
  private pageSize = DEFAULT_PAGE_SIZE;

  constructor(
    private fb: NonNullableFormBuilder,
    private orderService: OrderService,
    private clientService: ClientService,
    private alertService: AlertService,
    private dialogService: DialogService,
    private router: Router,
    private orderRoutingService: OrderRoutingService,
    private dialog: MatDialog,
    private _t: TranslationService
  ) {
    this.dataSource = new MatTableDataSource<OrderSummaryDto>();
    this.filterForm = this.fb.group({
      search: this.fb.control(''),
      clientId: new FormControl<string | null>(null),
      status: new FormControl<number | null>(null),
      isZeroOrder: new FormControl<boolean | null>(null),
      dateFrom: new FormControl<Date | null>(null),
      dateTo: new FormControl<Date | null>(null)
    });
  }

  async ngOnInit() {
    this.loadClients();
    await this.loadOrders();
  }

  private loadClients(): void {
    this.clientService.getAll().subscribe({
      next: (res) => {
        this.clientItems = (res.data ?? []).map(c => ({ id: c.id, name: c.name }));
      }
    });
  }

  private buildFilters() {
    const value = this.filterForm.getRawValue();
    return {
      search: value.search.trim() || undefined,
      clientId: value.clientId || undefined,
      status: value.status ?? undefined,
      isZeroOrder: value.isZeroOrder ?? undefined,
      dateFrom: this.toDateParam(value.dateFrom),
      dateTo: this.toDateParam(value.dateTo)
    };
  }

  private toDateParam(value: Date | null): string | undefined {
    if (!value) return undefined;
    const year = value.getFullYear();
    const month = String(value.getMonth() + 1).padStart(2, '0');
    const day = String(value.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private async loadOrders() {
    this.isLoading = true;
    try {
      const response = await firstValueFrom(
        this.orderService.getOrdersSummaryList(this.pageSize, this.pageNumber, this.buildFilters())
      );
      this.dataSource.data = response.data.items;
      this.totalCount = response.data.totalCount;
    } catch {
      this.alertService.showError('حدث خطأ أثناء تحميل الطلبات');
    } finally {
      this.isLoading = false;
    }
  }

  protected async onSearch(): Promise<void> {
    this.pageNumber = DEFAULT_PAGE_NUMBER;
    await this.loadOrders();
  }

  protected async onReset(): Promise<void> {
    this.filterForm.reset({
      search: '',
      clientId: null,
      status: null,
      isZeroOrder: null,
      dateFrom: null,
      dateTo: null
    });
    this.pageNumber = DEFAULT_PAGE_NUMBER;
    await this.loadOrders();
  }

  public async onPageChange(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    await this.loadOrders();
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
      } catch {
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
