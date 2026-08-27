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
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { OrderInventoryItemsReportService } from '../../services/order-inventory-items-report.service';
import { OrderInventoryItemsReportDto, InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../models/order-inventory-items-report.dto';

@Component({
  selector: 'app-order-inventory-items-report',
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
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './order-inventory-items-report.component.html',
  styleUrl: './order-inventory-items-report.component.scss',
})
export class OrderInventoryItemsReportComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  items: InventoryItemFilterDto[] = [];
  reportResult: OrderInventoryItemsReportDto | null = null;
  isLoading = false;

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    inventoryItemId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  displayedColumns = [
    'itemCategory', 'itemName', 'packsPerCarton', 'unitsPerPack',
    'cartonsIn', 'unitsIn', 'cartonsOut', 'unitsOut',
    'periodNetCartons', 'currentStockCartons',
    'paperUsedUnits', 'expectedWaste', 'difference',
  ];

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: OrderInventoryItemsReportService,
    private alertService: AlertService,
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
    this.reportService.getCategories().subscribe({
      next: (res) => { this.categories = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات'); },
    });

    this.filterForm.controls.categoryId.valueChanges.subscribe(categoryId => {
      this.items = [];
      this.filterForm.controls.inventoryItemId.setValue('');
      this.reportResult = null;
      if (categoryId != null) {
        this.reportService.getItemsByCategory(categoryId).subscribe({
          next: (res) => { this.items = res.data ?? []; },
          error: () => { this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون'); },
        });
      }
    });
  }

  onViewReport(): void {
    if (this.filterForm.invalid) {
      this.filterForm.markAllAsTouched();
      return;
    }

    const { inventoryItemId, dateFrom, dateTo } = this.filterForm.getRawValue();
    const from = this.asDate(dateFrom);
    const to = this.asDate(dateTo);

    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoading = true;
    this.reportResult = null;

    this.reportService.getReport(
      inventoryItemId,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.reportResult = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التقرير'); },
    });
  }

  reset(): void {
    const now = new Date();
    this.items = [];
    this.reportResult = null;
    this.filterForm.reset({
      categoryId: null,
      inventoryItemId: '',
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
    });
  }

  differenceClass(value: number): string {
    if (value < 0) return 'amt-out';
    if (value > 0) return 'amt-in';
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
