import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
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
import { InventoryStockBalanceReportService } from '../../services/inventory-stock-balance-report.service';
import { InventoryStockBalanceReportDto } from '../../models/inventory-stock-balance-report.dto';
import { InventoryCategoryFilterDto } from '../../models/order-inventory-items-report.dto';

@Component({
  selector: 'app-inventory-stock-balance-report',
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
  templateUrl: './inventory-stock-balance-report.component.html',
  styleUrl: './inventory-stock-balance-report.component.scss',
})
export class InventoryStockBalanceReportComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  report: InventoryStockBalanceReportDto | null = null;
  isLoading = false;

  columns = [
    'itemName', 'categoryName', 'packs', 'opening', 'periodIn', 'periodOut', 'closing', 'status',
  ];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: InventoryStockBalanceReportService,
    private alertService: AlertService,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
    });
  }

  ngOnInit(): void {
    this.reportService.getInventoryCategories().subscribe({
      next: (res) => { this.categories = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات'); },
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
    this.filterForm.reset({
      categoryId: null,
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
    });
    this.search();
  }

  stockClass(value: number): string {
    if (value < 0) return 'amt-out';
    if (value === 0) return 'amt-muted';
    return 'amt-balance';
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
