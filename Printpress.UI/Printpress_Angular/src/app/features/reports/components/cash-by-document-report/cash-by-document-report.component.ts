import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
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
import { CashAccountService } from '../../../general/services/cash-account.service';
import { CashAccountDto } from '../../../general/models/cash-account.dto';
import { CashByDocumentReportService } from '../../services/cash-by-document-report.service';
import { CashByDocumentReportDto, CashDocumentGroupDto } from '../../models/cash-by-document-report.dto';

@Component({
  selector: 'app-cash-by-document-report',
  standalone: true,
  imports: [
    CommonModule, RouterLink, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatIconModule, MatDatepickerModule, MatNativeDateModule,
    MatTableModule, MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './cash-by-document-report.component.html',
  styleUrl: './cash-by-document-report.component.scss',
})
export class CashByDocumentReportComponent implements OnInit {
  accounts: CashAccountDto[] = [];
  report: CashByDocumentReportDto | null = null;
  isLoading = false;
  columns = ['type', 'reference', 'transactionCount', 'totalIn', 'totalOut', 'net'];

  typeLabels: Record<string, string> = {
    None: 'بدون مرجع',
    Order: 'طلب',
    PurchaseInventoryInvoice: 'فاتورة مشتريات مخزون',
    PurchaseSparePartInvoice: 'فاتورة شراء قطع غيار',
    SellingSparePartInvoice: 'فاتورة بيع قطع غيار',
    WorkerSalaryTransaction: 'حركة راتب',
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
    private cashAccountService: CashAccountService,
    private reportService: CashByDocumentReportService,
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
    this.cashAccountService.getAll().subscribe({ next: (res) => { this.accounts = res.data ?? []; } });
    this.search();
  }

  get rows(): CashDocumentGroupDto[] {
    return this.report?.documents ?? [];
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

  typeLabel(row: CashDocumentGroupDto): string {
    const key = row.referenceType != null ? String(row.referenceType) : row.referenceTypeName;
    return this.typeLabels[key] ?? this.typeLabels[row.referenceTypeName] ?? key;
  }

  shortId(id: string | null): string {
    if (!id) return '—';
    return id.length > 8 ? id.slice(0, 8) + '…' : id;
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
