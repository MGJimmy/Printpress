import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { WorkerService } from '../../../hr/services/worker.service';
import { InventoryStockOutReportService } from '../../services/inventory-stock-out-report.service';
import { InventoryStockOutReportDto } from '../../models/inventory-stock-out-report.dto';
import { inventoryItemCategoryArabic } from '../../../inventory/pipes/inventory-item-category-arabic.pipe';
import { SearchSelectComponent, SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';

@Component({
  selector: 'app-inventory-stock-out-report',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    SearchSelectComponent,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './inventory-stock-out-report.component.html',
  styleUrl: './inventory-stock-out-report.component.scss',
})
export class InventoryStockOutReportComponent implements OnInit {
  categoryOptions: SearchSelectItem[] = [{ value: null, label: 'الكل' }];
  itemOptions: SearchSelectItem[] = [{ value: null, label: 'الكل' }];
  workerOptions: SearchSelectItem[] = [{ value: null, label: 'الكل' }];
  report: InventoryStockOutReportDto | null = null;
  isLoading = false;

  columns = ['movementDate', 'itemName', 'categoryName', 'quantity', 'workerName', 'notes', 'source'];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    inventoryItemId: FormControl<string | null>;
    workerId: FormControl<string | null>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: InventoryStockOutReportService,
    private workerService: WorkerService,
    private alertService: AlertService,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      inventoryItemId: this.fb.control<string | null>(null),
      workerId: this.fb.control<string | null>(null),
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
    });
  }

  ngOnInit(): void {
    this.reportService.getInventoryCategories().subscribe({
      next: (res) => {
        this.categoryOptions = [
          { value: null, label: 'الكل' },
          ...(res.data ?? []).map((category) => ({
            value: category.id,
            label: inventoryItemCategoryArabic(category.name),
          })),
        ];
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات'); },
    });

    this.workerService.getActive().subscribe({
      next: (res) => {
        this.workerOptions = [
          { value: null, label: 'الكل' },
          ...(res.data ?? []).map((worker) => ({ value: worker.id, label: worker.name })),
        ];
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل العمال'); },
    });

    this.filterForm.controls.categoryId.valueChanges.subscribe((categoryId) => {
      this.itemOptions = [{ value: null, label: 'الكل' }];
      this.filterForm.controls.inventoryItemId.setValue(null);
      if (categoryId != null) {
        this.reportService.getItemsByCategory(categoryId).subscribe({
          next: (res) => {
            this.itemOptions = [
              { value: null, label: 'الكل' },
              ...(res.data ?? []).map((item) => ({ value: item.id, label: item.name })),
            ];
          },
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
    this.reportService.getReport(
      v.categoryId,
      v.inventoryItemId || undefined,
      v.workerId || undefined,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.report = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التقرير'); },
    });
  }

  txQuery(row: { itemId: string }): Record<string, string> {
    const query: Record<string, string> = { itemId: row.itemId, type: 'Out' };
    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);
    if (from) query['dateFrom'] = this.toIsoDate(from);
    if (to) query['dateTo'] = this.toIsoDate(to);
    return query;
  }

  reset(): void {
    const now = new Date();
    this.itemOptions = [{ value: null, label: 'الكل' }];
    this.filterForm.reset({
      categoryId: null,
      inventoryItemId: null,
      workerId: null,
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
    });
    this.search();
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
