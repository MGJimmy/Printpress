import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
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
import { ZeroOrdersReportService } from '../../services/zero-orders-report.service';
import { ZeroOrdersReportDto } from '../../models/zero-orders-report.dto';
import { TranslationService } from '../../../../core/services/translation.service';
import { statusI18nKey } from '../../../order/models/enums/status-display';

@Component({
  selector: 'app-zero-orders-report',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './zero-orders-report.component.html',
  styleUrl: './zero-orders-report.component.scss',
})
export class ZeroOrdersReportComponent implements OnInit {
  report: ZeroOrdersReportDto | null = null;
  isLoading = false;
  columns = ['createdAt', 'orderName', 'clientName', 'status', 'serviceCount', 'itemCount'];

  filterForm: FormGroup<{
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private reportService: ZeroOrdersReportService,
    private alertService: AlertService,
    private translation: TranslationService,
  ) {
    this.filterForm = this.fb.group({
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
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
    this.reportService.getReport(
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.report = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل الطلبات الصفرية'); },
    });
  }

  reset(): void {
    this.filterForm.reset({ dateFrom: null, dateTo: null });
    this.search();
  }

  statusText(status: string): string {
    return this.translation.t(statusI18nKey(status));
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
