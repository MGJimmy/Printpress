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
import { PurchaseInvoiceService } from '../../services/purchase-invoice.service';
import { InventoryService } from '../../services/inventory.service';
import { InventoryPurchaseInvoiceListItemDto } from '../../models/inventory-document-list.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../../reports/models/order-inventory-items-report.dto';
import { ApiUrlResource } from '../../../../core/resources/api-urls.resource';
import { HttpService } from '../../../../core/services/http.service';
import { ApiResponseDto } from '../../../../core/models/api-response.dto';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-inventory-stock-in-invoices',
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
  templateUrl: './inventory-stock-in-invoices.component.html',
  styleUrl: '../inventory-docs.shared.scss',
})
export class InventoryStockInInvoicesComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  items: InventoryItemFilterDto[] = [];
  invoices: InventoryPurchaseInvoiceListItemDto[] = [];
  invoiceCount = 0;
  isLoading = false;
  displayedColumns = ['invoiceNumber', 'invoiceDate', 'supplierName', 'totalAmount', 'status', 'createdAt', 'action'];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    itemId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
    isVoided: FormControl<boolean | null>;
  }>;

  private pageNumber = DEFAULT_PAGE_NUMBER;
  private pageSize = DEFAULT_PAGE_SIZE;

  constructor(
    private fb: NonNullableFormBuilder,
    private invoiceService: PurchaseInvoiceService,
    private inventoryService: InventoryService,
    private http: HttpService,
    private alertService: AlertService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      itemId: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
      isVoided: this.fb.control<boolean | null>(null),
    });
  }

  ngOnInit(): void {
    const focusedId = this.route.snapshot.queryParamMap.get('invoiceId');
    if (focusedId) {
      this.router.navigate(['/inventory/stock-in/invoices', focusedId], { replaceUrl: true });
      return;
    }

    this.http.get<ApiResponseDto<InventoryCategoryFilterDto[]>>(ApiUrlResource.InventoryAPI.CategoryBasicInfoAll).subscribe({
      next: (res) => { this.categories = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات'); },
    });
    this.filterForm.controls.categoryId.valueChanges.subscribe((categoryId) => {
      this.items = [];
      this.filterForm.controls.itemId.setValue('');
      if (categoryId != null) {
        this.inventoryService.getByCategory(categoryId).subscribe({
          next: (res) => { this.items = (res.data ?? []).map((i) => ({ id: i.id, name: i.name })); },
          error: () => { this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون'); },
        });
      }
    });
    this.search();
  }

  search(): void {
    this.pageNumber = DEFAULT_PAGE_NUMBER;
    this.load();
  }

  reset(): void {
    this.items = [];
    this.filterForm.reset({
      categoryId: null,
      itemId: '',
      dateFrom: null,
      dateTo: null,
      isVoided: null,
    });
    this.search();
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.load();
  }

  viewInvoice(id: string): void {
    this.router.navigate(['/inventory/stock-in/invoices', id]);
  }

  goCreate(): void {
    this.router.navigate(['/inventory/stock-in']);
  }

  goItems(): void {
    this.router.navigate(['/inventory/items']);
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
      v.categoryId,
      v.itemId || undefined,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
      v.isVoided,
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
