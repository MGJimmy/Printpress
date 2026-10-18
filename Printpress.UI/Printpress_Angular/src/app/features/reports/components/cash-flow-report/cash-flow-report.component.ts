import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { CashAccountService } from '../../../general/services/cash-account.service';
import { CashFlowReportService } from '../../services/cash-flow-report.service';
import { CashFlowBucketDto, CashFlowReportDto } from '../../models/cash-flow-report.dto';
import { SearchSelectComponent, SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';

@Component({
  selector: 'app-cash-flow-report',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatDatepickerModule, MatNativeDateModule, SearchSelectComponent,
    MatTableModule, MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './cash-flow-report.component.html',
  styleUrl: './cash-flow-report.component.scss',
})
export class CashFlowReportComponent implements OnInit {
  accountOptions: SearchSelectItem[] = [{ value: null, label: 'الكل' }];
  report: CashFlowReportDto | null = null;
  isLoading = false;
  dayColumns = ['dayLabel', 'dayCount', 'dayIn', 'dayOut', 'dayNet', 'dayOpen'];
  monthColumns = ['monthLabel', 'monthCount', 'monthIn', 'monthOut', 'monthNet', 'monthOpen'];

  filterForm: FormGroup<{
    cashAccountId: FormControl<string | null>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private router: Router,
    private cashAccountService: CashAccountService,
    private reportService: CashFlowReportService,
    private alertService: AlertService,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      cashAccountId: this.fb.control<string | null>(null),
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
    });
  }

  ngOnInit(): void {
    this.cashAccountService.getAll().subscribe({
      next: (res) => {
        this.accountOptions = [
          { value: null, label: 'الكل' },
          ...(res.data ?? []).map((account) => ({ value: account.id, label: account.name })),
        ];
      },
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
      cashAccountId: null,
      dateFrom: new Date(now.getFullYear(), now.getMonth(), 1),
      dateTo: new Date(now.getFullYear(), now.getMonth(), now.getDate()),
    });
    this.search();
  }

  openBucket(row: CashFlowBucketDto): void {
    let from = row.key;
    let to = row.key;
    if (row.key.length === 7) {
      const [y, m] = row.key.split('-').map(Number);
      const last = new Date(y, m, 0).getDate();
      from = `${row.key}-01`;
      to = `${row.key}-${String(last).padStart(2, '0')}`;
    }
    this.router.navigate(['/reports/cash-book'], {
      queryParams: {
        cashAccountId: this.filterForm.controls.cashAccountId.value || undefined,
        dateFrom: from,
        dateTo: to,
      },
    });
  }

  private asDate(value: Date | null): Date | null {
    if (!value) return null;
    const date = value instanceof Date ? value : new Date(value);
    return Number.isNaN(date.getTime()) ? null : date;
  }

  private toIsoDate(date: Date): string {
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
  }
}
