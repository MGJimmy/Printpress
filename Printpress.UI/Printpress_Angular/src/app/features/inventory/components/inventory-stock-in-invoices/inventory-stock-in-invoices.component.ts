import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { ConfigurationService } from '../../../../core/services/configuration.service';
import { PurchaseInvoiceService } from '../../services/purchase-invoice.service';
import { InventoryService } from '../../services/inventory.service';
import { InventoryPurchaseInvoiceListDto } from '../../models/inventory-document-list.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../../reports/models/order-inventory-items-report.dto';
import { ApiUrlResource } from '../../../../core/resources/api-urls.resource';
import { HttpService } from '../../../../core/services/http.service';
import { ApiResponseDto } from '../../../../core/models/api-response.dto';

@Component({
  selector: 'app-inventory-stock-in-invoices',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
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
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './inventory-stock-in-invoices.component.html',
  styleUrl: '../inventory-docs.shared.scss',
})
export class InventoryStockInInvoicesComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  items: InventoryItemFilterDto[] = [];
  report: InventoryPurchaseInvoiceListDto | null = null;
  isLoading = false;
  focusedInvoiceId: string | null = null;
  lineColumns = ['itemName', 'categoryName', 'packsPerCarton', 'unitsPerPack', 'quantity', 'unitPrice', 'lineTotal'];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    itemId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private invoiceService: PurchaseInvoiceService,
    private inventoryService: InventoryService,
    private http: HttpService,
    private alertService: AlertService,
    private config: ConfigurationService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      itemId: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
    });
  }

  ngOnInit(): void {
    this.focusedInvoiceId = this.route.snapshot.queryParamMap.get('invoiceId');
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
    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);
    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoading = true;
    this.invoiceService.getAll(
      v.categoryId,
      v.itemId || undefined,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => {
        this.report = res.data;
        this.scrollToFocused();
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل فواتير الإدخال'); },
    });
  }

  reset(): void {
    this.items = [];
    this.filterForm.reset({
      categoryId: null,
      itemId: '',
      dateFrom: null,
      dateTo: null,
    });
    this.search();
  }

  openAttachment(path: string | null | undefined): void {
    if (!path) return;
    window.open(`${this.config.getConfiguration().apiUrl}${path}`, '_blank');
  }

  goCreate(): void {
    this.router.navigate(['/inventory/stock-in']);
  }

  goItems(): void {
    this.router.navigate(['/inventory/items']);
  }

  lineQtyTotal(lines: { quantity: number }[]): number {
    return lines.reduce((sum, line) => sum + (line.quantity || 0), 0);
  }

  private scrollToFocused(): void {
    if (!this.focusedInvoiceId) return;
    setTimeout(() => {
      document.querySelector('.invoice-card.focused')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
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
