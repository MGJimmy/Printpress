import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
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
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule
  ],
  templateUrl: './inventory-services-usage-report.component.html'
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
    'itemCategory', 'itemName', 'packsPerCarton', 'unitsPerPack',
    'cartonsIn', 'unitsIn', 'cartonsOut', 'unitsOut', 'expectedProductionWastePercent'
  ];

  serviceColumns = ['serviceName', 'orderCount', 'itemCount', 'paperUsed'];

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: InventoryServicesUsageReportService,
    private alertService: AlertService
  ) {
    this.filterForm = this.fb.group({
      inventoryItemCategoryId: new FormControl<number | null>(null, Validators.required),
      serviceCategoryId: this.fb.control('', Validators.required),
      dateFrom: new FormControl<Date | null>(null),
      dateTo: new FormControl<Date | null>(null)
    });
  }

  ngOnInit(): void {
    this.loadFilters();
  }

  private loadFilters(): void {
    this.reportService.getInventoryCategories().subscribe({
      next: (res) => { this.inventoryCategories = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل تصنيفات المخزون'); }
    });

    this.reportService.getServiceCategories().subscribe({
      next: (res) => { this.serviceCategories = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل تصنيفات الخدمات'); }
    });
  }

  onViewReport(): void {
    if (this.filterForm.invalid) {
      this.filterForm.markAllAsTouched();
      return;
    }

    const { inventoryItemCategoryId, serviceCategoryId, dateFrom, dateTo } = this.filterForm.getRawValue();
    this.isLoading = true;
    this.reportResult = null;

    this.reportService.getReport(
      inventoryItemCategoryId!,
      serviceCategoryId,
      dateFrom ? dateFrom.toISOString() : undefined,
      dateTo ? this.toEndOfDay(dateTo).toISOString() : undefined
    ).subscribe({
      next: (res) => {
        this.reportResult = res.data;
        this.isLoading = false;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل التقرير');
        this.isLoading = false;
      }
    });
  }

  private toEndOfDay(date: Date): Date {
    const d = new Date(date);
    d.setHours(23, 59, 59, 999);
    return d;
  }
}
