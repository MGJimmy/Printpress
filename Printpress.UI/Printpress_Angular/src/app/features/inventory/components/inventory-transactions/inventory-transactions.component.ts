import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
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
import { InventoryTransactionService } from '../../services/inventory-transaction.service';
import { InventoryService } from '../../services/inventory.service';
import { WorkerService } from '../../../hr/services/worker.service';
import { WorkerDto } from '../../../hr/models/worker.dto';
import { InventoryTransactionListDto } from '../../models/inventory-document-list.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../../reports/models/order-inventory-items-report.dto';
import { ApiUrlResource } from '../../../../core/resources/api-urls.resource';
import { HttpService } from '../../../../core/services/http.service';
import { ApiResponseDto } from '../../../../core/models/api-response.dto';

@Component({
  selector: 'app-inventory-transactions',
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
  templateUrl: './inventory-transactions.component.html',
  styleUrl: '../inventory-docs.shared.scss',
})
export class InventoryTransactionsComponent implements OnInit {
  categories: InventoryCategoryFilterDto[] = [];
  items: InventoryItemFilterDto[] = [];
  workers: WorkerDto[] = [];
  report: InventoryTransactionListDto | null = null;
  isLoading = false;
  columns = ['createdAt', 'type', 'itemName', 'categoryName', 'quantity', 'reference', 'workerName', 'notes'];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    itemId: FormControl<string>;
    workerId: FormControl<string>;
    type: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private transactionService: InventoryTransactionService,
    private inventoryService: InventoryService,
    private workerService: WorkerService,
    private http: HttpService,
    private alertService: AlertService,
    private router: Router,
  ) {
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      itemId: this.fb.control(''),
      workerId: this.fb.control(''),
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
    this.workerService.getActive().subscribe({
      next: (res) => { this.workers = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل العمال'); },
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
    this.transactionService.getAll(
      v.categoryId,
      v.itemId || undefined,
      v.workerId || undefined,
      v.type || undefined,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.report = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل حركات المخزن'); },
    });
  }

  reset(): void {
    this.items = [];
    this.filterForm.reset({
      categoryId: null,
      itemId: '',
      workerId: '',
      type: '',
      dateFrom: null,
      dateTo: null,
    });
    this.search();
  }

  typeLabel(type: string): string {
    if (type === 'In') return 'دخول';
    if (type === 'Out') return 'خروج';
    if (type === 'Adjustment') return 'تسوية';
    return type;
  }

  typeClass(type: string): string {
    if (type === 'In') return 'badge-in';
    if (type === 'Out') return 'badge-out';
    return 'badge-adj';
  }

  qtyClass(type: string): string {
    return type === 'Out' ? 'amt-out' : 'amt-in';
  }

  goCreateOut(): void {
    this.router.navigate(['/inventory/stock-out']);
  }

  goItems(): void {
    this.router.navigate(['/inventory/items']);
  }

  openReference(route: string): void {
    void this.router.navigateByUrl(route);
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
