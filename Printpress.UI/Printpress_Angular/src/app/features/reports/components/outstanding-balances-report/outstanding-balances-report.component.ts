import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { OutstandingBalancesReportService } from '../../services/outstanding-balances-report.service';
import {
  OutstandingBalanceKind,
  OutstandingBalanceRowDto,
  OutstandingBalancesReportDto,
} from '../../models/outstanding-balances-report.dto';

@Component({
  selector: 'app-outstanding-balances-report',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './outstanding-balances-report.component.html',
  styleUrl: './outstanding-balances-report.component.scss',
})
export class OutstandingBalancesReportComponent implements OnInit {
  report: OutstandingBalancesReportDto | null = null;
  isLoading = false;
  columns = ['direction', 'type', 'partyName', 'documentLabel', 'date', 'total', 'paid', 'remaining'];

  constructor(
    private reportService: OutstandingBalancesReportService,
    private alertService: AlertService,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.reportService.getReport().pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => { this.report = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل المستحقات'); },
    });
  }

  directionText(direction: string): string {
    return direction === 'Collect' ? 'تحصيل' : 'دفع';
  }

  typeText(type: OutstandingBalanceKind): string {
    switch (type) {
      case 'Order': return 'طلب';
      case 'WorkerAdvance': return 'سلفة عامل';
      case 'PurchaseInvoice': return 'فاتورة مخزن';
      case 'SparePartPurchaseInvoice': return 'فاتورة قطع غيار';
      case 'Loan': return 'قرض';
      case 'MonthlySalary': return 'راتب شهري';
      default: return type;
    }
  }

  amountText(value: number | null | undefined): string {
    return value == null ? '—' : value.toLocaleString('en-US');
  }

  documentText(row: OutstandingBalanceRowDto): string {
    return row.documentLabel || '—';
  }
}
