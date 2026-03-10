import { Component, Input, OnInit, Injector } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderSellingItemGetDto } from '../../models/orderSellingItem/order-selling-item-get.dto';
import { OrderSellingItemUpsertComponent } from '../order-selling-item-upsert/order-selling-item-upsert.component';
import { AlertService } from '../../../../core/services/alert.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { ConfirmDialogModel } from '../../../../core/models/confirm-dialog.model';

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

  displayedColumns = ['name', 'type', 'quantity', 'price', 'total', 'actions'];
  dataSource: OrderSellingItemGetDto[] = [];

  constructor(
    private orderSharedDataService: OrderSharedDataService,
    private dialog: MatDialog,
    private alertService: AlertService,
    private dialogService: DialogService,
    private injector: Injector
  ) { }

  ngOnInit(): void {
    this.bindData();
  }

  private bindData(): void {
    this.dataSource = this.orderSharedDataService.getOrderSellingItems_copy();
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
}
