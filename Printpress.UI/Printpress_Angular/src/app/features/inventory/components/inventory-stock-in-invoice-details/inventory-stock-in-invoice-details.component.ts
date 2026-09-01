import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { ConfigurationService } from '../../../../core/services/configuration.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { PurchaseInvoiceService } from '../../services/purchase-invoice.service';
import { InventoryPurchaseInvoiceListItemDto } from '../../models/inventory-document-list.dto';

@Component({
  selector: 'app-inventory-stock-in-invoice-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './inventory-stock-in-invoice-details.component.html',
  styleUrl: '../inventory-docs.shared.scss',
})
export class InventoryStockInInvoiceDetailsComponent implements OnInit {
  invoice: InventoryPurchaseInvoiceListItemDto | null = null;
  isLoading = false;
  isVoiding = false;
  isPaying = false;
  isReceiving = false;
  payAmount: number | null = null;
  payNote = '';
  lineColumns = ['itemName', 'categoryName', 'packsPerCarton', 'unitsPerPack', 'quantity', 'unitPrice', 'lineTotal'];
  paymentColumns = ['transactionDate', 'amount', 'description', 'status'];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private invoiceService: PurchaseInvoiceService,
    private alertService: AlertService,
    private config: ConfigurationService,
    private dialogService: DialogService,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/inventory/stock-in/invoices']);
      return;
    }
    this.load(id);
  }

  goBack(): void {
    this.router.navigate(['/inventory/stock-in/invoices']);
  }

  openAttachment(path: string | null | undefined): void {
    if (!path) return;
    window.open(`${this.config.getConfiguration().apiUrl}${path}`, '_blank');
  }

  lineQtyTotal(lines: { quantity: number }[]): number {
    return lines.reduce((sum, line) => sum + (line.quantity || 0), 0);
  }

  payRemaining(): void {
    if (!this.invoice || this.invoice.isVoided || this.invoice.remainingAmount <= 0) return;
    const amount = this.payAmount ?? 0;
    if (amount <= 0) {
      this.alertService.showError('أدخل مبلغاً أكبر من صفر');
      return;
    }
    if (amount > this.invoice.remainingAmount) {
      this.alertService.showError('المبلغ أكبر من الباقي على الفاتورة');
      return;
    }
    this.isPaying = true;
    this.invoiceService.pay(this.invoice.id, amount, this.payNote).pipe(
      finalize(() => { this.isPaying = false; }),
    ).subscribe({
      next: () => {
        this.alertService.showSuccess('تم تسجيل الدفعة');
        this.payAmount = null;
        this.payNote = '';
        this.load(this.invoice!.id);
      },
    });
  }

  receiveGoods(): void {
    if (!this.invoice || this.invoice.isVoided || this.invoice.isGoodsReceived) return;
    this.dialogService.confirmDialog({
      title: 'استلام للمخزن',
      message: 'سيتم إدخال كميات هذه الفاتورة إلى المخزن. متابعة؟',
      confirmText: 'نعم، استلام',
      cancelText: 'تراجع',
    }).subscribe((ok) => {
      if (!ok) return;
      this.isReceiving = true;
      this.invoiceService.receive(this.invoice!.id).pipe(
        finalize(() => { this.isReceiving = false; }),
      ).subscribe({
        next: () => {
          this.alertService.showSuccess('تم إدخال الأصناف إلى المخزن');
          this.load(this.invoice!.id);
        },
      });
    });
  }

  voidInvoice(): void {
    if (!this.invoice || this.invoice.isVoided) return;
    const stockMsg = this.invoice.isGoodsReceived
      ? 'سيتم عكس كميات المخزن وحركات الخزينة المرتبطة بهذه الفاتورة.'
      : 'سيتم عكس حركات الخزينة المرتبطة بهذه الفاتورة. الأصناف لم تدخل المخزن.';
    this.dialogService.promptDialog({
      title: 'تأكيد إلغاء الفاتورة',
      message: `${stockMsg} أدخل سبب الإلغاء للمتابعة.`,
      fieldLabel: 'سبب الإلغاء',
      confirmText: 'نعم، إلغاء',
      cancelText: 'تراجع',
      maxLength: 500,
    }).subscribe((reason) => {
      if (!reason) return;
      this.isVoiding = true;
      this.invoiceService.void(this.invoice!.id, reason).pipe(
        finalize(() => { this.isVoiding = false; }),
      ).subscribe({
        next: () => {
          this.alertService.showSuccess('تم إلغاء الفاتورة');
          this.load(this.invoice!.id);
        },
      });
    });
  }

  private load(id: string): void {
    this.isLoading = true;
    this.invoiceService.getById(id).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => {
        this.invoice = res.data;
        this.invoice.payments ??= [];
        this.invoice.lines ??= [];
        if (this.invoice.remainingAmount > 0) {
          this.payAmount = this.invoice.remainingAmount;
        }
      },
      error: () => {
        this.alertService.showError('تعذر تحميل تفاصيل الفاتورة');
        this.goBack();
      },
    });
  }
}
