import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
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
import { InventoryService } from '../../services/inventory.service';
import { InventoryUsageSettlementService } from '../../services/inventory-usage-settlement.service';
import { InventoryUsageSettlementListDto, InventoryUsageSettlementType } from '../../models/inventory-usage-settlement.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../../reports/models/order-inventory-items-report.dto';
import { ApiUrlResource } from '../../../../core/resources/api-urls.resource';
import { HttpService } from '../../../../core/services/http.service';
import { ApiResponseDto } from '../../../../core/models/api-response.dto';

@Component({
  selector: 'app-usage-settlement-list',
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
  templateUrl: './usage-settlement-list.component.html',
  styleUrl: '../inventory-docs.shared.scss',
})
export class UsageSettlementListComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  items: InventoryItemFilterDto[] = [];
  report: InventoryUsageSettlementListDto | null = null;
  isLoading = false;
  columns = ['occurredAt', 'type', 'itemName', 'categoryName', 'quantity', 'notes', 'createdAt'];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    itemId: FormControl<string>;
    type: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private settlementService: InventoryUsageSettlementService,
    private inventoryService: InventoryService,
    private http: HttpService,
    private alertService: AlertService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      itemId: this.fb.control(''),
      type: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
    });
  }

  ngOnInit(): void {
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

    const q = this.route.snapshot.queryParamMap;
    const itemId = q.get('itemId');
    const dateFrom = q.get('dateFrom');
    const dateTo = q.get('dateTo');
    if (dateFrom) {
      const d = new Date(dateFrom);
      if (!Number.isNaN(d.getTime())) this.filterForm.controls.dateFrom.setValue(d);
    }
    if (dateTo) {
      const d = new Date(dateTo);
      if (!Number.isNaN(d.getTime())) this.filterForm.controls.dateTo.setValue(d);
    }
    if (itemId) {
      this.filterForm.controls.itemId.setValue(itemId);
    }

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
    this.settlementService.getAll(
      v.categoryId,
      v.itemId || undefined,
      v.type || undefined,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.report = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التسويات'); },
    });
  }

  reset(): void {
    this.items = [];
    this.filterForm.reset({
      categoryId: null,
      itemId: '',
      type: '',
      dateFrom: null,
      dateTo: null,
    });
    this.search();
  }

  typeLabel(type: InventoryUsageSettlementType): string {
    if (type === 'ExtraWaste') return 'هالك إضافي';
    if (type === 'Theft') return 'سرقة / فقد';
    if (type === 'Other') return 'أخرى';
    return type;
  }

  goCreate(): void {
    const itemId = this.filterForm.controls.itemId.value;
    this.router.navigate(['/inventory/usage-settlements/new'], {
      queryParams: itemId ? { itemId } : {},
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
