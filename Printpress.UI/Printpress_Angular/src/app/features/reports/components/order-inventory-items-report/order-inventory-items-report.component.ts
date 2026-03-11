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
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule
  ],
  templateUrl: './order-inventory-items-report.component.html',
  styleUrls: ['./order-inventory-items-report.component.scss']
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
    'paperUsedUnits', 'expectedWaste', 'difference'
  ];

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: OrderInventoryItemsReportService,
    private alertService: AlertService
  ) {
    this.filterForm = this.fb.group({
      categoryId: new FormControl<number | null>(null, Validators.required),
      inventoryItemId: this.fb.control('', Validators.required),
      dateFrom: new FormControl<Date | null>(null),
      dateTo: new FormControl<Date | null>(null)
    });
  }

  ngOnInit(): void {
    this.reportService.getCategories().subscribe({
      next: (res) => { this.categories = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات'); }
    });

    this.filterForm.controls.categoryId.valueChanges.subscribe(categoryId => {
      this.items = [];
      this.filterForm.controls.inventoryItemId.setValue('');
      if (categoryId != null) {
        this.reportService.getItemsByCategory(categoryId).subscribe({
          next: (res) => { this.items = res.data; },
          error: () => { this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون'); }
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
    this.isLoading = true;
    this.reportResult = null;

    this.reportService.getReport(
      inventoryItemId,
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
