import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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
import { CashAccountService } from '../../../general/services/cash-account.service';
import { CashAccountDto } from '../../../general/models/cash-account.dto';
import { CashMovementSummaryReportService } from '../../services/cash-movement-summary-report.service';
import { CashMovementSliceDto, CashMovementSummaryReportDto } from '../../models/cash-movement-summary-report.dto';

@Component({
  selector: 'app-cash-movement-summary-report',
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
  templateUrl: './cash-movement-summary-report.component.html',
  styleUrl: './cash-movement-summary-report.component.scss',
})
export class CashMovementSummaryReportComponent implements OnInit {
  accounts: CashAccountDto[] = [];
  report: CashMovementSummaryReportDto | null = null;
  isLoading = false;

  categoryColumns = ['catLabel', 'catCount', 'catIn', 'catOut', 'catNet', 'catOpen'];
  accountColumns = ['accLabel', 'accCount', 'accIn', 'accOut', 'accNet', 'accOpen'];

  categoryLabels: Record<string, string> = {
    Sales: 'مبيعات',
    SalesReturn: 'مرتجع مبيعات',
    Purchases: 'مشتريات',
    Expenses: 'مصروفات',
    CapitalInjection: 'ضخ رأس المال',
    Maintenance: 'صيانة',
    ExternalServices: 'خدمات خارجية',
    Salaries: 'رواتب',
    Transfer: 'تحويل',
    Other: 'أخرى',
  };

  filterForm: FormGroup<{
    cashAccountId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private cashAccountService: CashAccountService,
    private reportService: CashMovementSummaryReportService,
    private alertService: AlertService,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      cashAccountId: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
    });
  }

  ngOnInit(): void {
    const preselected = this.route.snapshot.queryParamMap.get('cashAccountId');
    if (preselected) this.filterForm.controls.cashAccountId.setValue(preselected);

    this.cashAccountService.getAll().subscribe({
      next: (res) => { this.accounts = res.data ?? []; },
    });
    this.search();
  }

  get byCategory(): CashMovementSliceDto[] {
    return this.report?.byCategory ?? [];
  }

  get byAccount(): CashMovementSliceDto[] {
    return this.report?.byAccount ?? [];
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
    this.reportService.getReport({
      cashAccountId: v.cashAccountId || undefined,
      dateFrom: from ? this.toIsoDate(from) : undefined,
      dateTo: to ? this.toIsoDate(to) : undefined,
    }).pipe(finalize(() => { this.isLoading = false; })).subscribe({
      next: (res) => { this.report = res.data; },
    });
  }

  reset(): void {
    const now = new Date();
    this.filterForm.reset({
      cashAccountId: '',
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
    });
    this.search();
  }

  categoryLabel(row: CashMovementSliceDto): string {
    const key = row.category != null ? String(row.category) : row.key;
    return this.categoryLabels[key] ?? key;
  }

  openCategory(row: CashMovementSliceDto): void {
    this.openCashBook({ category: String(row.category ?? row.key) });
  }

  openAccount(row: CashMovementSliceDto): void {
    this.openCashBook({ cashAccountId: row.cashAccountId ?? row.key });
  }

  private openCashBook(extra: { category?: string; cashAccountId?: string }): void {
    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);
    this.router.navigate(['/reports/cash-book'], {
      queryParams: {
        cashAccountId: extra.cashAccountId || v.cashAccountId || undefined,
        category: extra.category,
        dateFrom: from ? this.toIsoDate(from) : undefined,
        dateTo: to ? this.toIsoDate(to) : undefined,
      },
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
