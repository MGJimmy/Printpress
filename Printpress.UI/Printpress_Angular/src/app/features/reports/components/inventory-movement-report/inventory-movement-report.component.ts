import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
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
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { InventoryMovementReportService } from '../../services/inventory-movement-report.service';
import { InventoryMovementReportDto } from '../../models/inventory-movement-report.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../models/order-inventory-items-report.dto';

@Component({
  selector: 'app-inventory-movement-report',
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
    RouterLink,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './inventory-movement-report.component.html',
  styleUrl: './inventory-movement-report.component.scss',
})
export class InventoryMovementReportComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  items: InventoryItemFilterDto[] = [];
  report: InventoryMovementReportDto | null = null;
  isLoading = false;
  private hydrating = false;

  columns = ['movementDate', 'type', 'referenceType', 'inQuantity', 'outQuantity', 'runningBalance', 'workerName', 'notes'];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    inventoryItemId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: InventoryMovementReportService,
    private alertService: AlertService,
    private route: ActivatedRoute,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      categoryId: new FormControl<number | null>(null, Validators.required),
      inventoryItemId: this.fb.control('', Validators.required),
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
    });
  }

  ngOnInit(): void {
    this.reportService.getInventoryCategories().subscribe({
      next: (res) => { this.categories = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات'); },
    });

    this.filterForm.controls.categoryId.valueChanges.subscribe((categoryId) => {
      if (this.hydrating) return;
      this.items = [];
      this.filterForm.controls.inventoryItemId.setValue('');
      this.report = null;
      if (categoryId != null) {
        this.reportService.getItemsByCategory(categoryId).subscribe({
          next: (res) => { this.items = res.data ?? []; },
          error: () => { this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون'); },
        });
      }
    });

    this.hydrateFromQuery();
  }

  sourcePath(route: string): string {
    return route.split('?')[0];
  }

  sourceQuery(route: string): Record<string, string> {
    const i = route.indexOf('?');
    if (i < 0) return {};
    const params: Record<string, string> = {};
    new URLSearchParams(route.substring(i + 1)).forEach((value, key) => {
      params[key] = value;
    });
    return params;
  }

  private hydrateFromQuery(): void {
    const q = this.route.snapshot.queryParamMap;
    const categoryRaw = q.get('categoryId');
    const inventoryItemId = q.get('inventoryItemId');
    const dateFrom = this.parseIsoDate(q.get('dateFrom'));
    const dateTo = this.parseIsoDate(q.get('dateTo'));
    if (dateFrom) this.filterForm.controls.dateFrom.setValue(dateFrom);
    if (dateTo) this.filterForm.controls.dateTo.setValue(dateTo);

    const categoryId = categoryRaw ? Number(categoryRaw) : NaN;
    if (!Number.isFinite(categoryId) || !inventoryItemId) return;

    this.hydrating = true;
    this.filterForm.controls.categoryId.setValue(categoryId);
    this.reportService.getItemsByCategory(categoryId).subscribe({
      next: (res) => {
        this.items = res.data ?? [];
        this.filterForm.controls.inventoryItemId.setValue(inventoryItemId);
        this.hydrating = false;
        this.search();
      },
      error: () => {
        this.hydrating = false;
        this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون');
      },
    });
  }

  private parseIsoDate(value: string | null): Date | null {
    if (!value) return null;
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? null : date;
  }

  search(): void {
    if (this.filterForm.invalid) {
      this.filterForm.markAllAsTouched();
      return;
    }

    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);
    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoading = true;
    this.reportService.getReport(
      v.inventoryItemId,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.report = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التقرير'); },
    });
  }

  reset(): void {
    const now = new Date();
    this.items = [];
    this.report = null;
    this.filterForm.reset({
      categoryId: null,
      inventoryItemId: '',
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
    });
  }

  typeClass(type: string): string {
    if (type === 'دخول') return 'badge-in';
    if (type === 'خروج') return 'badge-out';
    return 'badge-adj';
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
