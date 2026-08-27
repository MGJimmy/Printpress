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
import { InventoryServicesUsageReportService } from '../../services/inventory-services-usage-report.service';
import {
  InventoryServicesUsageReportDto,
  ServiceCategoryFilterDto
} from '../../models/inventory-services-usage-report.dto';
import { InventoryCategoryFilterDto } from '../../models/order-inventory-items-report.dto';

@Component({
  selector: 'app-inventory-services-usage-report',
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
  templateUrl: './inventory-services-usage-report.component.html',
  styleUrl: './inventory-services-usage-report.component.scss',
})
export class InventoryServicesUsageReportComponent implements OnInit {
  inventoryCategories: InventoryCategoryFilterDto[] = [];
  serviceCategories: ServiceCategoryFilterDto[] = [];
  reportResult: InventoryServicesUsageReportDto | null = null;
  isLoading = false;

  filterForm: FormGroup<{
    inventoryItemCategoryId: FormControl<number | null>;
    serviceCategoryId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  inventoryColumns = [
    'invCategory', 'invName', 'invPacks', 'invUnitsPerPack',
    'invIn', 'invUnitsIn', 'invOut', 'invUnitsOut',
    'invNet', 'invStock', 'invWaste',
  ];

  serviceColumns = ['svcName', 'svcOrders', 'svcItems', 'svcPaper'];

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: InventoryServicesUsageReportService,
    private alertService: AlertService,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      inventoryItemCategoryId: new FormControl<number | null>(null, Validators.required),
      serviceCategoryId: this.fb.control('', Validators.required),
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
    });
  }

  ngOnInit(): void {
    this.loadFilters();
  }

  private loadFilters(): void {
    this.reportService.getInventoryCategories().subscribe({
      next: (res) => { this.inventoryCategories = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل تصنيفات المخزون'); },
    });

    this.reportService.getServiceCategories().subscribe({
      next: (res) => { this.serviceCategories = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل تصنيفات الخدمات'); },
    });
  }

  onViewReport(): void {
    if (this.filterForm.invalid) {
      this.filterForm.markAllAsTouched();
      return;
    }

    const { inventoryItemCategoryId, serviceCategoryId, dateFrom, dateTo } = this.filterForm.getRawValue();
    const from = this.asDate(dateFrom);
    const to = this.asDate(dateTo);

    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoading = true;
    this.reportResult = null;

    this.reportService.getReport(
      inventoryItemCategoryId!,
      serviceCategoryId,
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
    this.reportResult = null;
    this.filterForm.reset({
      inventoryItemCategoryId: null,
      serviceCategoryId: '',
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
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
