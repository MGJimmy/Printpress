import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { OrderService } from '../../services/order.service';
import { TranslationService } from '../../../../core/services/translation.service';
import { AlertService } from '../../../../core/services/alert.service';
import { OrderGroupGetDto } from '../../models/orderGroup/order-group-get.Dto';

export interface InvoiceGroupSelectDialogData {
  orderId: string;
}

interface InvoiceGroupOption {
  id: string;
  name: string;
  status?: string;
  selected: boolean;
}

@Component({
  selector: 'app-invoice-group-select-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatCheckboxModule
  ],
  templateUrl: './invoice-group-select-dialog.component.html',
  styleUrl: './invoice-group-select-dialog.component.css'
})
export class InvoiceGroupSelectDialogComponent implements OnInit {
  groups: InvoiceGroupOption[] = [];
  isLoading = true;

  constructor(
    private orderService: OrderService,
    private alertService: AlertService,
    public _t: TranslationService,
    public dialogRef: MatDialogRef<InvoiceGroupSelectDialogComponent, string[] | undefined>,
    @Inject(MAT_DIALOG_DATA) public data: InvoiceGroupSelectDialogData
  ) {}

  async ngOnInit(): Promise<void> {
    try {
      const response = await firstValueFrom(this.orderService.getOrderById(this.data.orderId));
      const orderGroups: OrderGroupGetDto[] = response?.data?.orderGroups ?? [];
      this.groups = orderGroups.map(g => ({
        id: g.id,
        name: g.name,
        status: g.status,
        selected: true
      }));
    } catch {
      this.alertService.showError(this._t.t('orders.error_loading_groups'));
      this.dialogRef.close();
    } finally {
      this.isLoading = false;
    }
  }

  get allSelected(): boolean {
    return this.groups.length > 0 && this.groups.every(g => g.selected);
  }

  get someSelected(): boolean {
    return this.groups.some(g => g.selected) && !this.allSelected;
  }

  get hasSelection(): boolean {
    return this.groups.some(g => g.selected);
  }

  toggleAll(checked: boolean): void {
    this.groups.forEach(g => g.selected = checked);
  }

  getStatusText(status?: string): string {
    switch (status) {
      case 'New':        return this._t.t('orders.status_new');
      case 'InProgress': return this._t.t('orders.status_in_progress');
      case 'Completed':  return this._t.t('orders.status_completed');
      case 'Delivered':  return this._t.t('orders.status_delivered');
      default:           return this._t.t('orders.status_unknown');
    }
  }

  onConfirm(): void {
    if (!this.hasSelection) {
      return;
    }

    this.dialogRef.close(this.groups.filter(g => g.selected).map(g => g.id));
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
