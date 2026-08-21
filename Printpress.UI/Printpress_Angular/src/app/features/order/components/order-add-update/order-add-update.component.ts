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
import { OrderRoutingService } from '../../services/order-routing.service';
import { GroupDeleveryPopupComponent } from '../popup/group-delevery-popup/group-delevery-popup.component';
import { OrderServicesGetDTO } from '../../models/order-service/order-service-getDto';
import { mapOrderGetToUpsert } from '../../models/order-mapper';
import { ObjectStateEnum } from '../../../../core/models/object-state.enum';
import { TranslationService } from '../../../../core/services/translation.service';
import { isStatus, normalizeStatus, statusBadgeClass, statusI18nKey } from '../../models/enums/status-display';

@Component({
  selector: 'app-order-add-update',
  standalone: true,
  imports: imports,
  templateUrl: './order-add-update.component.html',
  styleUrl: './order-add-update.component.css',
})
export class OrderAddUpdateComponent implements OnInit, OnDestroy {

  public componentMode: ComponentMode;
  public displayedColumns = ['name', 'executionType', 'status', 'deliveryDate', 'deliveredTo', 'action'];
  public executionTypeLabels: Record<string, string> = {
    Internal: 'داخلي',
    External_WithOurMaterials: 'خارجي (بموادنا)',
    External_Full: 'خارجي (كامل)'
  };
  public orderGroupGridDataSource !: MatTableDataSource<OrderGroupGridViewModel>;
  public groupStatusFilter: string = 'all';
  private allGroupRows: OrderGroupGridViewModel[] = [];
  public get groupStatusLabels(): Record<string, string> {
    return {
      New: this._t.t('orders.status_new'),
      InProgress: this._t.t('orders.status_in_progress'),
      Completed: this._t.t('orders.status_completed'),
      Delivered: this._t.t('orders.status_delivered')
    };
  }

  public get orderStatusLabel(): string {
    return this._t.t(statusI18nKey(this.orderGetDto?.status));
  }

  public get orderStatusCardClass(): string {
    const status = normalizeStatus(this.orderGetDto?.status);
    return status === 'InProgress' ? 'bg-warning text-dark' : `${statusBadgeClass(status)} text-white`;
  }

  protected groupStatusLabel(status?: string | number): string {
    const normalized = normalizeStatus(status);
    return this.groupStatusLabels[normalized] || normalized;
  }

  protected groupStatusBadgeClass(status?: string | number): string {
    return statusBadgeClass(status);
  }
  public clients: ClientGetDto[] = [];
  public orderClientId!: string
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
    private dialogService: DialogService,
    private orderRoutingService: OrderRoutingService,
    private _t: TranslationService
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
     
      let orderId = this.activedRoute.snapshot.paramMap.get('id');

      if (this.orderGetDto.id != orderId) {
        let response = await firstValueFrom(this.orderService.getOrderById(orderId!));
        this.orderGetDto = response.data
        this.OrderSharedService.setOrderObject(this.orderGetDto);
      }
    }

    // Temp solution to prevent unsaved changes warning in case of view mode
    // This should be handled better in the future
    if (this.componentMode.isViewMode){
      this.OrderSharedService.onOrderSaved();
    }

    this.bindGroups();
    this.orderName = this.orderGetDto.name;
    this.orderClientId = this.orderGetDto.clientId;

    await this.loadAllClients();

  }

  private bindGroups(){
    this.allGroupRows = this.MapToOrderGroupGridViewModel(this.OrderSharedService.getOrderGroups_Copy());
    this.applyGroupFilter();
  }

  public applyGroupFilter(): void {
    const filtered = this.groupStatusFilter === 'all'
      ? this.allGroupRows
      : this.allGroupRows.filter(g => normalizeStatus(g.status) === this.groupStatusFilter);
    this.orderGroupGridDataSource = new MatTableDataSource<OrderGroupGridViewModel>(filtered);
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
      width: '1000px',
      injector: this.injector,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe();
  }

  protected onDeleteGroup(groupId: string) {
    const group = this.OrderSharedService.getOrderGroups_Copy().find(g => g.id === groupId);
    if (group && this.groupHasExecutedItems(group)) {
      this.alertService.showError(this._t.t('orders.cannot_delete_group_with_executions'));
      return;
    }

    const dialogData: ConfirmDialogModel = {
      title: this._t.t('orders.confirm_delete'),
      message: this._t.t('orders.delete_group_msg'),
      confirmText: this._t.t('shared.yes'),
      cancelText: this._t.t('shared.cancel'),
    };

    this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {

        this.OrderSharedService.deleteGroup(groupId);

        this.bindGroups();

        this.alertService.showSuccess(this._t.t('orders.group_deleted'));
      }
    });
  }

  protected onAddGroup() {
    this.router.navigate([this.orderRoutingService.getGroupAddRoute()]);
  }

  protected canEditGroup(group: { status?: string; deliveryDate?: Date }): boolean {
    return !this.isGroupClosed(group);
  }

  protected canDeleteGroup(group: OrderGroupGridViewModel): boolean {
    return !this.isGroupClosed(group) && !group.hasExecutedItems;
  }

  protected canDeliverGroup(group: { status?: string; deliveryDate?: Date }): boolean {
    return isStatus(group.status, 'Completed') && !group.deliveryDate;
  }

  private isGroupClosed(group: { status?: string; deliveryDate?: Date }): boolean {
    return isStatus(group.status, 'Completed', 'Delivered') || !!group.deliveryDate;
  }

  private groupHasExecutedItems(group: OrderGroupGetDto): boolean {
    return (group.items ?? []).some(item => item.hasExecutions === true || isStatus(item.status, 'Completed'));
  }

  protected onEditGroup(groupId: string) {
    this.router.navigate([this.orderRoutingService.getGroupRoute(groupId)]);
  }

  protected onViewGroupItems(groupId: string) {
    this.router.navigate([this.orderRoutingService.getGroupItemsRoute(groupId)]);
  }

  public saveOrder_Click() {
    if (!this.validateOrderData()) {
      return;
    }

    const dialogRef = this.dialog.open(OrderServicePricesComponent, {
      data: { orderSharedService: this.OrderSharedService },
      width: '1000px'
    });

    dialogRef.afterClosed().subscribe((orderServices: OrderServicesGetDTO[] | undefined) => {
      if (orderServices) {
        this.OrderSharedService.setOrderServices(orderServices);
        this.OrderSharedService.updateOrderObjectState();
        
        const orderDTO = this.OrderSharedService.getOrderObject_copy(true);
        const orderUpsertDTO = mapOrderGetToUpsert(orderDTO);

        let upsertObservable = orderDTO.objectState == ObjectStateEnum.added || orderDTO.objectState == ObjectStateEnum.temp ?
          this.orderService.insertOrder(orderUpsertDTO) :
          this.orderService.updateOrder(orderUpsertDTO);

          upsertObservable.subscribe({
            next: (response) => {
              this.OrderSharedService.onOrderSaved();
              this.alertService.showSuccess(this._t.t('orders.order_saved'));
              this.router.navigate([this.orderRoutingService.getOrderListRoute()]);
            },
            error: (error) => {
              this.alertService.showError(this._t.t('orders.error_saving_order'));
            }
          });
          
      }
    });
    
  }



  private validateOrderData(): boolean {


    debugger;
    const order= this.OrderSharedService.getOrderObject_copy()
    const emptyGroupsList = order.orderGroups.length == 0;
    if (emptyGroupsList) {
      this.alertService.showError(this._t.t('orders.groups_required'));
      return false;
    }
    if(!order.name){
      this.alertService.showError(this._t.t('orders.order_name_required'))
      return false;
    }
    if(!order.clientId){
      this.alertService.showError(this._t.t('orders.client_required'))
      return false;
    }

    return true;
  }

  public openAddClientDialog() {
    const clientDialog = this.dialog.open(AddClientComponent, {
      width: '600px'
    });

    clientDialog.afterClosed().subscribe((clientId: string) => {
      this.loadAllClients();
      this.orderClientId = clientId;
      this.onClientSelectChange();
    });

  }

  private MapToOrderGroupGridViewModel(orderGroupGetDtos: OrderGroupGetDto[]): OrderGroupGridViewModel[] {
    if (!orderGroupGetDtos) {
      return [];
    }

    return orderGroupGetDtos.map((orderGroup) => {
      return {
        id: orderGroup.id,
        name: orderGroup.name,
        executionType: orderGroup.executionType,
        status: normalizeStatus(orderGroup.status),
        deliveryDate: orderGroup.deliveryDate,
        deliveredTo: orderGroup.deliveredTo,
        deliveryNotes: orderGroup.deliveryNotes,
        hasExecutedItems: this.groupHasExecutedItems(orderGroup)
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
    this.router.navigate([this.orderRoutingService.getOrderListRoute()]);
  }

  public onDeleverGroup(groupId: string) {
    const dialogRef = this.dialog.open(GroupDeleveryPopupComponent, {
      width: '500px',
      disableClose: true,
      data : {
        isViewMode: false
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {

        result.id = groupId;

        this.orderService.deliverOrderGroup(result).subscribe((response) => { 

          let orderId = this.activedRoute.snapshot.paramMap.get('id');

          if (orderId) {
              this.orderService.getOrderById(orderId).subscribe(res=>{
              this.orderGetDto = res.data
              this.OrderSharedService.setOrderObject(this.orderGetDto);      
              this.bindGroups()
              this.alertService.showSuccess(this._t.t('orders.group_delivered'));
            })}
        })   
      }
    });
  }

  public onDeliveryDetails(groupId: string){
    const group = this.OrderSharedService.getOrderGroups_Copy().find(g => g.id === groupId);
    if (!group) return;

    const dialogRef = this.dialog.open(GroupDeleveryPopupComponent, {
      width: '500px',
      disableClose: true,
      data: {
        deliveryDate: group.deliveryDate,
        deliveredFrom: group.deliveredFrom,
        deliveredTo: group.deliveredTo,
        deliveryNotes: group.deliveryNotes,
        isViewMode: true
      }
    });

    dialogRef.afterClosed().subscribe();
  }
  
}

interface OrderGroupGridViewModel {
  id: string;
  name: string;
  executionType?: string;
  status?: string;
  deliveryDate?: Date;
  deliveredTo?: string;
  deliveryNotes?: string;
  hasExecutedItems: boolean;
}




