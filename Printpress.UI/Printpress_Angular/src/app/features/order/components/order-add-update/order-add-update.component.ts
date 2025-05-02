import { Component, Injector, OnInit, OnDestroy } from '@angular/core';
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
import { firstValueFrom, Subject, takeUntil } from 'rxjs';
import { OrderGroupGetDto } from '../../models/orderGroup/order-group-get.Dto';
import { TransactionComponent } from '../transaction/transaction.component';
import { OrderCommunicationService } from '../../services/order-communication.service';
import { OrderEventType } from '../../models/enums/order-events.enum';
import { ConfirmDialogModel } from '../../../../core/models/confirm-dialog.model';
import { DialogService } from '../../../../shared/services/dialog.service';

@Component({
  selector: 'app-order-add-update',
  standalone: true,
  imports: imports,
  templateUrl: './order-add-update.component.html',
  styleUrl: './order-add-update.component.css',
})
export class OrderAddUpdateComponent implements OnInit, OnDestroy {

  public componentMode: ComponentMode;
  public displayedColumns = ['name', 'deliveryDate', 'action'];
  public orderGroupGridDataSource !: MatTableDataSource<OrderGroupGridViewModel>;
  public clients: ClientGetDto[] = [];
  public orderClientId!: number
  public orderName!: string;
  public orderGetDto: OrderGetDto;
  private destroy$ = new Subject<void>();

  constructor(private router: Router,
    private OrderSharedService: OrderSharedDataService,
    private injector: Injector,
    private clientService: ClientService,
    private dialog: MatDialog,
    private alertService: AlertService,
    private activedRoute: ActivatedRoute,
    private orderService: OrderService,
    private orderComm: OrderCommunicationService,
    private dialogService: DialogService
  ) {
    this.componentMode = new ComponentMode(this.router);
    this.orderGetDto = this.OrderSharedService.getOrderObject_copy();

    this.orderComm.on(OrderEventType.ORDER_MAINDATA_UPDATED)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.orderService.getOrderMainData(this.orderGetDto.id).subscribe((response) => {
          OrderSharedService.refreshOrderMainData(response.data);
          this.orderGetDto = this.OrderSharedService.getOrderObject_copy();
        });
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  async ngOnInit() {

    if (this.componentMode.isViewMode || this.componentMode.isEditMode) {
      if (this.componentMode.isViewMode) {
        this.displayedColumns = this.displayedColumns.filter(col => col !== 'action')
      }
      let orderId = Number(this.activedRoute.snapshot.paramMap.get('id'));

      if (this.orderGetDto.id != orderId) {
        let response = await firstValueFrom(this.orderService.getOrderById(orderId));
        this.orderGetDto = response.data
        this.OrderSharedService.setOrderObject(this.orderGetDto);
      }
    }

    this.bindGroups();
    this.orderName = this.orderGetDto.name;
    this.orderClientId = this.orderGetDto.clientId;

    await this.loadAllClients();

  }

  private bindGroups(){
    this.orderGroupGridDataSource = new MatTableDataSource<OrderGroupGridViewModel>(this.MapToOrderGroupGridViewModel(this.OrderSharedService.getOrderGroups_Copy()));
  }

  async loadAllClients() {
    let response = await firstValueFrom(this.clientService.getAll())
    this.clients = response.data;
  }

  protected manageTransactions_Click(): void {
    this.openTransactionModal();
  }

  private openTransactionModal() {
    let dialogRef = this.dialog.open(TransactionComponent, {
      data: { orderId: this.OrderSharedService.getOrderObject_copy().id },
      height: '550px',
      width: '1000px',
      injector: this.injector,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe();
  }

  protected onDeleteGroup(groupId: number) {
    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذه المجموعة؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {

        this.OrderSharedService.deleteGroup(groupId);

        this.bindGroups();

        this.alertService.showSuccess('تم حذف المجموعة بنجاح!');
      }
    });
  }

  protected onEditGroup(groupId: number) {
    this.router.navigate([this.OrderSharedService.getGroupRoute(groupId)]);
  }

  public saveOrder_Click() {

    if (!this.validateOrderData()) {
      return;
    }

    this.openServicePricesDialog();
  }

  private validateOrderData(): boolean {
    const emptyGroupsList = this.OrderSharedService.getOrderObject_copy().orderGroups.length == 0;
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

  public openAddClientDialog() {
    const clientDialog = this.dialog.open(AddClientComponent, {
      width: '600px'
    });

    clientDialog.afterClosed().subscribe((clientId: number) => {
      this.loadAllClients();
      this.orderClientId = clientId;
      this.onClientSelectChange();
    });

  }

  private MapToOrderGroupGridViewModel(orderGroupGetDtos: OrderGroupGetDto[]): OrderGroupGridViewModel[] {
    if (!orderGroupGetDtos) {
      return [];
    }

    return orderGroupGetDtos.map((orderGroup, index) => {
      return {
        id: orderGroup.id,
        name: orderGroup.name,
        deliveryDate: orderGroup.deliveryDate
      }
    });
  }

  protected onOrderNameInputBlur() {
    this.OrderSharedService.setOrderName(this.orderName);
  }

  protected onClientSelectChange() {
    this.OrderSharedService.setOrderClient(this.orderClientId);
  }

  public goBack() {
    this.router.navigate(['/orderlist']);
  }
}

interface OrderGroupGridViewModel {
  name: string;
  deliveryDate?: Date;
}



