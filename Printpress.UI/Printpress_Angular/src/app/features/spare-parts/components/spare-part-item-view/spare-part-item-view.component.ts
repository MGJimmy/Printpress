import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageEvent } from '@angular/material/paginator';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { SparePartService } from '../../services/spare-part.service';
import { SparePartTransactionService } from '../../services/spare-part-transaction.service';
import { SparePartItemDto } from '../../models/spare-part-item.dto';
import { SparePartTransactionDto } from '../../models/spare-part-transaction.dto';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-spare-part-item-view',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatTableModule,
    MatProgressSpinnerModule,
    SharedPaginationComponent,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './spare-part-item-view.component.html',
  styleUrl: './spare-part-item-view.component.scss',
})
export class SparePartItemViewComponent implements OnInit {
  item: SparePartItemDto | null = null;
  transactions: SparePartTransactionDto[] = [];
  totalTransactionsCount = 0;
  isLoadingItem = false;
  isLoadingTx = false;
  displayedColumns = ['createdAt', 'type', 'quantity', 'notes'];

  filterForm: FormGroup<{
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
    transactionType: FormControl<string>;
  }>;

  private itemId!: string;
  private pageNumber = DEFAULT_PAGE_NUMBER;
  private pageSize = DEFAULT_PAGE_SIZE;

  constructor(
    private fb: NonNullableFormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private sparePartService: SparePartService,
    private sparePartTransactionService: SparePartTransactionService,
  ) {
    this.filterForm = this.fb.group({
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
      transactionType: this.fb.control(''),
    });
  }

  ngOnInit(): void {
    this.itemId = this.activatedRoute.snapshot.paramMap.get('id')!;
    this.loadItem();
    this.loadTransactions();
  }

  search(): void {
    this.pageNumber = DEFAULT_PAGE_NUMBER;
    this.loadTransactions();
  }

  reset(): void {
    this.filterForm.reset({
      dateFrom: null,
      dateTo: null,
      transactionType: '',
    });
    this.search();
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.loadTransactions();
  }

  onBack(): void {
    this.router.navigate(['/spare-parts/items']);
  }

  onEdit(): void {
    this.router.navigate(['/spare-parts/items/edit', this.itemId]);
  }

  goStockIn(): void {
    this.router.navigate(['/spare-parts/stock-in']);
  }

  goStockOut(): void {
    this.router.navigate(['/spare-parts/stock-out']);
  }

  typeLabel(type: string): string {
    if (type === 'In') return 'إدخال';
    if (type === 'Out') return 'صرف';
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

  private loadItem(): void {
    this.isLoadingItem = true;
    this.sparePartService.getById(this.itemId).pipe(
      finalize(() => { this.isLoadingItem = false; }),
    ).subscribe({
      next: (response) => { this.item = response.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل بيانات قطعة الغيار'); },
    });
  }

  private loadTransactions(): void {
    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);
    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoadingTx = true;
    this.sparePartTransactionService.getByItemId(
      this.itemId,
      this.pageNumber,
      this.pageSize,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
      v.transactionType || undefined,
    ).pipe(
      finalize(() => { this.isLoadingTx = false; }),
    ).subscribe({
      next: (response) => {
        this.transactions = response.data.items as SparePartTransactionDto[];
        this.totalTransactionsCount = response.data.totalCount;
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل حركات قطع الغيار'); },
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
