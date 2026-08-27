import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
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
import { CashTreasuryReportService } from '../../services/cash-treasury-report.service';
import { CashTreasuryReportDto } from '../../models/cash-treasury-report.dto';

@Component({
  selector: 'app-cash-treasury-report',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatDatepickerModule, MatNativeDateModule, MatTableModule,
    MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './cash-treasury-report.component.html',
  styleUrl: './cash-treasury-report.component.scss',
})
export class CashTreasuryReportComponent implements OnInit {
  report: CashTreasuryReportDto | null = null;
  isLoading = false;
  inColumns = ['inDate', 'inAccount', 'inAmount', 'inCategory', 'inDesc'];
  outColumns = ['outDate', 'outAccount', 'outAmount', 'outCategory', 'outDesc'];
  transferColumns = ['trDate', 'trFrom', 'trTo', 'trAmount', 'trStatus', 'trDesc'];

  filterForm: FormGroup<{
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: CashTreasuryReportService,
    private alertService: AlertService,
  ) {
    const now = new Date();
    this.filterForm = this.fb.group({
      dateFrom: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), 1)),
      dateTo: this.fb.control<Date | null>(new Date(now.getFullYear(), now.getMonth(), now.getDate())),
    });
  }

  ngOnInit(): void {
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
      dateFrom: from ? this.toIsoDate(from) : undefined,
      dateTo: to ? this.toIsoDate(to) : undefined,
    }).pipe(finalize(() => { this.isLoading = false; })).subscribe({
      next: (res) => { this.report = res.data; },
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
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
  }
}
