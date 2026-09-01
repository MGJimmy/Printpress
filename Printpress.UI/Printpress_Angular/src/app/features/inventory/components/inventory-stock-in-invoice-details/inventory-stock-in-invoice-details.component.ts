import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
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
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './inventory-stock-in-invoice-details.component.html',
  styleUrl: '../inventory-docs.shared.scss',
})
export class InventoryStockInInvoiceDetailsComponent implements OnInit {
  invoice: InventoryPurchaseInvoiceListItemDto | null = null;
  isLoading = false;
  isVoiding = false;
  lineColumns = ['itemName', 'categoryName', 'packsPerCarton', 'unitsPerPack', 'quantity', 'unitPrice', 'lineTotal'];

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

  voidInvoice(): void {
    if (!this.invoice || this.invoice.isVoided) return;
    this.dialogService.promptDialog({
      title: 'تأكيد إلغاء الفاتورة',
      message: 'سيتم عكس كميات المخزن وحركة الخزينة المرتبطة بهذه الفاتورة. أدخل سبب الإلغاء للمتابعة.',
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
      next: (res) => { this.invoice = res.data; },
      error: () => {
        this.alertService.showError('تعذر تحميل تفاصيل الفاتورة');
        this.goBack();
      },
    });
  }
}
