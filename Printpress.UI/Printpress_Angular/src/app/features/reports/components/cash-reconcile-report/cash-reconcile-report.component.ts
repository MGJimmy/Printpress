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
import { CashReconcileReportService } from '../../services/cash-reconcile-report.service';
import { CashReconcileAccountDto, CashReconcileReportDto } from '../../models/cash-reconcile-report.dto';

@Component({
  selector: 'app-cash-reconcile-report',
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
  templateUrl: './cash-reconcile-report.component.html',
  styleUrl: './cash-reconcile-report.component.scss',
})
export class CashReconcileReportComponent implements OnInit {
  accounts: CashAccountDto[] = [];
  report: CashReconcileReportDto | null = null;
  isLoading = false;

  columns = [
    'cashAccountName', 'storedBalance', 'computedBalance', 'difference',
    'match', 'openingBalance', 'periodIn', 'periodOut', 'periodClosing', 'periodCheck',
  ];

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
    private reportService: CashReconcileReportService,
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
    this.route.queryParamMap.subscribe((params) => {
      const preselected = params.get('cashAccountId');
      if (preselected) {
        this.filterForm.controls.cashAccountId.setValue(preselected);
      }
    });

    this.cashAccountService.getAll().subscribe({
      next: (res) => { this.accounts = res.data ?? []; },
    });

    this.search();
  }

  get rows(): CashReconcileAccountDto[] {
    return this.report?.accounts ?? [];
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
    }).pipe(
      finalize(() => { this.isLoading = false; })
    ).subscribe({
      next: (res) => { this.report = res.data; },
    });
  }

  reset(): void {
    const now = new Date();
    this.filterForm.reset({
      cashAccountId: this.route.snapshot.queryParamMap.get('cashAccountId') ?? '',
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
    });
    this.search();
  }

  openCashBook(row: CashReconcileAccountDto): void {
    const v = this.filterForm.getRawValue();
    this.router.navigate(['/reports/cash-book'], {
      queryParams: {
        cashAccountId: row.cashAccountId,
        dateFrom: this.asDate(v.dateFrom) ? this.toIsoDate(this.asDate(v.dateFrom)!) : undefined,
        dateTo: this.asDate(v.dateTo) ? this.toIsoDate(this.asDate(v.dateTo)!) : undefined,
      },
    });
  }

  typeLabel(type: string): string {
    if (type === 'SpareParts') return 'قطع غيار';
    if (type === 'Main') return 'رئيسية';
    return type;
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
