import { Component, EventEmitter, Input, OnInit, Output, Injector } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { finalize } from 'rxjs';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderSellingItemGetDto } from '../../models/orderSellingItem/order-selling-item-get.dto';
import { OrderSellingItemUpsertComponent } from '../order-selling-item-upsert/order-selling-item-upsert.component';
import { GroupDeleveryPopupComponent } from '../popup/group-delevery-popup/group-delevery-popup.component';
import { OrderService } from '../../services/order.service';
import { OrderGetDto } from '../../models/order/order-get.Dto';
import { AlertService } from '../../../../core/services/alert.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { ConfirmDialogModel } from '../../../../core/models/confirm-dialog.model';
import { TranslationService } from '../../../../core/services/translation.service';

@Component({
  selector: 'app-order-selling-items',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatDialogModule
  ],
  templateUrl: './order-selling-items.component.html',
  styleUrl: './order-selling-items.component.css'
})
export class OrderSellingItemsComponent implements OnInit {

  @Input() isViewMode: boolean = false;
  @Input() isOrderClosed: boolean = false;
  @Output() delivered = new EventEmitter<OrderGetDto>();

  displayedColumns = ['name', 'type', 'quantity', 'price', 'total', 'actions'];
  dataSource: OrderSellingItemGetDto[] = [];
  isDelivering = false;

  constructor(
    private orderSharedDataService: OrderSharedDataService,
    private dialog: MatDialog,
    private alertService: AlertService,
    private dialogService: DialogService,
    private injector: Injector,
    private orderService: OrderService,
    private route: ActivatedRoute,
    private translation: TranslationService
  ) { }

  ngOnInit(): void {
    this.bindData();
  }

  private bindData(): void {
    this.dataSource = this.orderSharedDataService.getOrderSellingItems_copy();
  }

  get sellingLinesTotal(): number {
    return this.dataSource.reduce((sum, item) => sum + (item.price || 0) * (item.quantity || 0), 0);
  }

  canDeliver(item: OrderSellingItemGetDto): boolean {
    return this.isViewMode && !this.isOrderClosed && !item.isDelivered;
  }

  canEditOrDelete(item: OrderSellingItemGetDto): boolean {
    return !this.isViewMode && !item.isDelivered;
  }

  onAddItem(): void {
    const dialogRef = this.dialog.open(OrderSellingItemUpsertComponent, {
      width: '600px',
      disableClose: true,
      injector: this.injector,
      data: {}
    });

    dialogRef.afterClosed().subscribe((saved: boolean) => {
      if (saved) {
        this.bindData();
        this.alertService.showSuccess('تم إضافة العنصر بنجاح');
      }
    });
  }

  onEditItem(item: OrderSellingItemGetDto): void {
    if (!this.canEditOrDelete(item)) return;

    const dialogRef = this.dialog.open(OrderSellingItemUpsertComponent, {
      width: '600px',
      disableClose: true,
      injector: this.injector,
      data: { itemId: item.id }
    });

    dialogRef.afterClosed().subscribe((saved: boolean) => {
      if (saved) {
        this.bindData();
        this.alertService.showSuccess('تم تعديل العنصر بنجاح');
      }
    });
  }

  onDeleteItem(item: OrderSellingItemGetDto): void {
    if (!this.canEditOrDelete(item)) return;

    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذا العنصر؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {
        this.orderSharedDataService.deleteSellingItem(item.id);
        this.bindData();
        this.alertService.showSuccess('تم حذف العنصر بنجاح');
      }
    });
  }

  onDeliver(item: OrderSellingItemGetDto): void {
    const dialogRef = this.dialog.open(GroupDeleveryPopupComponent, {
      width: '500px',
      disableClose: true,
      data: { isViewMode: false }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      result.id = item.id;
      this.isDelivering = true;
      this.orderService.deliverOrderSellingItem(result).pipe(
        finalize(() => { this.isDelivering = false; }),
      ).subscribe({
        next: () => this.reloadOrder(),
        error: () => {
          this.alertService.showError(this.translation.t('orders.error_executing'));
        }
      });
    });
  }

  onDeliveryDetails(item: OrderSellingItemGetDto): void {
    this.dialog.open(GroupDeleveryPopupComponent, {
      width: '500px',
      disableClose: true,
      data: {
        deliveryDate: item.deliveryDate,
        deliveredFrom: item.deliveryName,
        deliveredTo: item.receiverName,
        deliveryNotes: item.deliveryNotes,
        isViewMode: true
      }
    });
  }

  private reloadOrder(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    if (!orderId) return;

    this.orderService.getOrderById(orderId).subscribe({
      next: (res) => {
        this.orderSharedDataService.setOrderObject(res.data);
        this.bindData();
        this.delivered.emit(res.data);
        this.alertService.showSuccess(this.translation.t('orders.group_delivered'));
      }
    });
  }
}
