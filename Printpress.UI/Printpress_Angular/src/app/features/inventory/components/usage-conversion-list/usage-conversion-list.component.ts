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
import { DialogService } from '../../../../shared/services/dialog.service';
import { InventoryService } from '../../services/inventory.service';
import { InventoryConversionService } from '../../services/inventory-conversion.service';
import { InventoryConversionListDto, InventoryConversionStatus } from '../../models/inventory-conversion.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../../reports/models/order-inventory-items-report.dto';
import { ApiUrlResource } from '../../../../core/resources/api-urls.resource';
import { HttpService } from '../../../../core/services/http.service';
import { ApiResponseDto } from '../../../../core/models/api-response.dto';

@Component({
  selector: 'app-usage-conversion-list',
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
  templateUrl: './usage-conversion-list.component.html',
  styleUrl: '../inventory-docs.shared.scss',
})
export class UsageConversionListComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  items: InventoryItemFilterDto[] = [];
  report: InventoryConversionListDto | null = null;
  isLoading = false;
  isWorking = false;
  columns = ['occurredAt', 'status', 'itemName', 'categoryName', 'quantity', 'notes', 'createdAt', 'action'];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    itemId: FormControl<string>;
    status: FormControl<string>;
    isVoided: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private conversionService: InventoryConversionService,
    private inventoryService: InventoryService,
    private http: HttpService,
    private alertService: AlertService,
    private dialogService: DialogService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      itemId: this.fb.control(''),
      status: this.fb.control(''),
      isVoided: this.fb.control(''),
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
    const voidedFilter = v.isVoided === 'true' ? true : v.isVoided === 'false' ? false : null;
    this.conversionService.getAll(
      v.categoryId,
      v.itemId || undefined,
      v.status || undefined,
      voidedFilter,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.report = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التحويل إلى منتج'); },
    });
  }

  reset(): void {
    this.items = [];
    this.filterForm.reset({
      categoryId: null,
      itemId: '',
      status: '',
      isVoided: '',
      dateFrom: null,
      dateTo: null,
    });
    this.search();
  }

  statusLabel(status: InventoryConversionStatus): string {
    if (status === 'Open') return 'مفتوح';
    if (status === 'Completed') return 'مكتمل';
    return status;
  }

  goCreate(): void {
    const itemId = this.filterForm.controls.itemId.value;
    this.router.navigate(['/inventory/usage-conversions/new'], {
      queryParams: itemId ? { itemId } : {},
    });
  }

  completeRow(id: string, status: InventoryConversionStatus, isVoided: boolean): void {
    if (isVoided || status !== 'Open' || this.isWorking) return;
    this.isWorking = true;
    this.conversionService.complete(id).pipe(
      finalize(() => { this.isWorking = false; }),
    ).subscribe({
      next: () => {
        this.alertService.showSuccess('تم إكمال التحويل إلى منتج');
        this.search();
      },
    });
  }

  voidRow(id: string, isVoided: boolean): void {
    if (isVoided || this.isWorking) return;
    this.dialogService.promptDialog({
      title: 'تأكيد إلغاء التحويل إلى منتج',
      message: 'لن يُحسب هذا التحويل إلى منتج في تقرير الاستهلاك. أدخل سبب الإلغاء للمتابعة.',
      fieldLabel: 'سبب الإلغاء',
      confirmText: 'نعم، إلغاء',
      cancelText: 'تراجع',
      maxLength: 500,
    }).subscribe((reason) => {
      if (!reason) return;
      this.isWorking = true;
      this.conversionService.void(id, reason).pipe(
        finalize(() => { this.isWorking = false; }),
      ).subscribe({
        next: () => {
          this.alertService.showSuccess('تم إلغاء التحويل إلى منتج');
          this.search();
        },
      });
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
