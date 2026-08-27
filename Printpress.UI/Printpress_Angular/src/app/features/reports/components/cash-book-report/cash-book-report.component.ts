import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { CashAccountService } from '../../../general/services/cash-account.service';
import { CashAccountDto } from '../../../general/models/cash-account.dto';
import { CashBookReportService } from '../../services/cash-book-report.service';
import { CashBookLineDto, CashBookReportDto } from '../../models/cash-book-report.dto';

@Component({
  selector: 'app-cash-book-report',
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
    MatPaginatorModule,
    MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './cash-book-report.component.html',
  styleUrl: './cash-book-report.component.scss',
})
export class CashBookReportComponent implements OnInit {
  accounts: CashAccountDto[] = [];
  report: CashBookReportDto | null = null;
  isLoading = false;
  pageIndex = 0;
  pageSize = 10;
  pageSizeOptions = [5, 10, 25, 50];

  typeOptions = [
    { value: '', label: 'الكل' },
    { value: 'In', label: 'وارد' },
    { value: 'Out', label: 'صادر' },
  ];

  categoryOptions = [
    { value: '', label: 'الكل' },
    { value: 'Sales', label: 'مبيعات' },
    { value: 'SalesReturn', label: 'مرتجع مبيعات' },
    { value: 'Purchases', label: 'مشتريات' },
    { value: 'Expenses', label: 'مصروفات' },
    { value: 'CapitalInjection', label: 'ضخ رأس المال' },
    { value: 'Maintenance', label: 'صيانة' },
    { value: 'ExternalServices', label: 'خدمات خارجية' },
    { value: 'Salaries', label: 'رواتب' },
    { value: 'Transfer', label: 'تحويل' },
    { value: 'Other', label: 'أخرى' },
  ];

  lineColumns = [
    'transactionDate', 'lineAccountName', 'inAmount', 'outAmount',
    'runningBalance', 'category', 'reference', 'status', 'description', 'createdBy',
  ];

  summaryColumns = ['summaryAccountName', 'summaryOpening', 'summaryIn', 'summaryOut', 'summaryClosing'];

  filterForm: FormGroup<{
    cashAccountId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
    type: FormControl<string>;
    category: FormControl<string>;
    search: FormControl<string>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private cashAccountService: CashAccountService,
    private reportService: CashBookReportService,
    private alertService: AlertService,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      cashAccountId: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
      type: this.fb.control(''),
      category: this.fb.control(''),
      search: this.fb.control(''),
    });
  }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      const preselected = params.get('cashAccountId');
      if (preselected) {
        this.filterForm.controls.cashAccountId.setValue(preselected);
      }
      const from = this.parseIsoDate(params.get('dateFrom'));
      if (from) this.filterForm.controls.dateFrom.setValue(from);
      const to = this.parseIsoDate(params.get('dateTo'));
      if (to) this.filterForm.controls.dateTo.setValue(to);
      const category = params.get('category');
      if (category) this.filterForm.controls.category.setValue(category);
      const type = params.get('type');
      if (type) this.filterForm.controls.type.setValue(type);
    });

    this.cashAccountService.getAll().subscribe({
      next: (res) => { this.accounts = res.data ?? []; },
    });

    this.search();
  }

  get lines(): CashBookLineDto[] {
    return this.report?.lines ?? [];
  }

  get summaries() {
    return this.report?.accountSummaries ?? [];
  }

  get totalLines(): number {
    return this.report?.totalLineCount ?? 0;
  }

  search(): void {
    this.pageIndex = 0;
    this.load();
  }

  onPage(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.load();
  }

  load(): void {
    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);

    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoading = true;
    this.reportService.getReport({
      cashAccountId: v.cashAccountId || undefined,
      dateFrom: from ? this.toIsoDate(from) : undefined,
      dateTo: to ? this.toIsoDate(to) : undefined,
      type: v.type || undefined,
      category: v.category || undefined,
      search: v.search?.trim() || undefined,
      page: this.pageIndex + 1,
      pageSize: this.pageSize,
    }).pipe(
      finalize(() => { this.isLoading = false; })
    ).subscribe({
      next: (res) => {
        this.report = res.data;
        if (this.report?.page) {
          this.pageIndex = this.report.page - 1;
          this.pageSize = this.report.pageSize || this.pageSize;
        }
      },
    });
  }

  reset(): void {
    const now = new Date();
    this.filterForm.reset({
      cashAccountId: this.route.snapshot.queryParamMap.get('cashAccountId') ?? '',
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
      type: '',
      category: '',
      search: '',
    });
    this.search();
  }

  categoryLabel(category: string | number): string {
    const value = String(category);
    return this.categoryOptions.find((c) => c.value === value)?.label ?? value;
  }

  statusLabel(status: string): string {
    if (status === 'Voided') return 'ملغاة';
    if (status === 'Reversal') return 'عكس';
    return 'عادية';
  }

  statusClass(status: string): string {
    if (status === 'Voided') return 'badge-voided';
    if (status === 'Reversal') return 'badge-reversal';
    return 'badge-normal';
  }

  rowClass(row: CashBookLineDto): string {
    if (row.status === 'Voided') return 'row-voided';
    if (row.status === 'Reversal') return 'row-reversal';
    return '';
  }

  private parseIsoDate(value: string | null): Date | null {
    if (!value) return null;
    const parts = value.split('-').map(Number);
    if (parts.length !== 3 || parts.some((n) => Number.isNaN(n))) return null;
    return new Date(parts[0], parts[1] - 1, parts[2]);
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
