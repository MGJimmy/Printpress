import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageEvent } from '@angular/material/paginator';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { SparePartPurchaseInvoiceService } from '../../services/spare-part-purchase-invoice.service';
import { SparePartService } from '../../services/spare-part.service';
import { SparePartItemDto } from '../../models/spare-part-item.dto';
import { SparePartPurchaseInvoiceListItemDto } from '../../models/spare-part-invoice-list.dto';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-spare-parts-stock-in-invoices',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatProgressSpinnerModule,
    SharedPaginationComponent,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './spare-parts-stock-in-invoices.component.html',
  styleUrl: '../spare-part-invoice-list.shared.scss',
})
export class SparePartsStockInInvoicesComponent implements OnInit {
  items: SparePartItemDto[] = [];
  invoices: SparePartPurchaseInvoiceListItemDto[] = [];
  invoiceCount = 0;
  isLoading = false;
  displayedColumns = ['invoiceNumber', 'invoiceDate', 'supplierName', 'totalAmount', 'paidAmount', 'remainingAmount', 'status', 'createdAt', 'action'];

  filterForm: FormGroup<{
    itemId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
    isVoided: FormControl<boolean | null>;
    hasRemaining: FormControl<boolean | null>;
    isGoodsReceived: FormControl<boolean | null>;
  }>;

  private pageNumber = DEFAULT_PAGE_NUMBER;
  private pageSize = DEFAULT_PAGE_SIZE;

  constructor(
    private fb: NonNullableFormBuilder,
    private invoiceService: SparePartPurchaseInvoiceService,
    private sparePartService: SparePartService,
    private alertService: AlertService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.filterForm = this.fb.group({
      itemId: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
      isVoided: this.fb.control<boolean | null>(null),
      hasRemaining: this.fb.control<boolean | null>(null),
      isGoodsReceived: this.fb.control<boolean | null>(null),
    });
  }

  ngOnInit(): void {
    const focusedId = this.route.snapshot.queryParamMap.get('invoiceId');
    if (focusedId) {
      this.router.navigate(['/spare-parts/stock-in/invoices', focusedId], { replaceUrl: true });
      return;
    }

    this.sparePartService.getAllForSelection().subscribe({
      next: (res) => { this.items = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل قطع الغيار'); },
    });
    this.search();
  }

  search(): void {
    this.pageNumber = DEFAULT_PAGE_NUMBER;
    this.load();
  }

  reset(): void {
    this.filterForm.reset({
      itemId: '',
      dateFrom: null,
      dateTo: null,
      isVoided: null,
      hasRemaining: null,
      isGoodsReceived: null,
    });
    this.search();
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.load();
  }

  viewInvoice(id: string): void {
    this.router.navigate(['/spare-parts/stock-in/invoices', id]);
  }

  goCreate(): void {
    this.router.navigate(['/spare-parts/stock-in']);
  }

  goItems(): void {
    this.router.navigate(['/spare-parts/items']);
  }

  private load(): void {
    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);
    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoading = true;
    this.invoiceService.getAll(
      this.pageNumber,
      this.pageSize,
      v.itemId || undefined,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
      v.isVoided,
      v.hasRemaining,
      v.isGoodsReceived,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => {
        this.invoices = res.data?.invoices ?? [];
        this.invoiceCount = res.data?.invoiceCount ?? 0;
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل فواتير الإدخال'); },
    });
  }

  private asDate(value: Date | null): Date | null {
    if (!value) return null;
    const date = value instanceof Date ? value : new Date(value);
    return Number.isNaN(date.getTime()) ? null : date;
  }

  private toIsoDate(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }
}
