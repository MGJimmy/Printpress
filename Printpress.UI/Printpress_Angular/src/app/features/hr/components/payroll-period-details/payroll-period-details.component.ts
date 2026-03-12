import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { PayrollPeriodService } from '../../services/payroll-period.service';
import { PayrollPeriodDetailsDto, SalaryTransactionTypeLabels } from '../../models/payroll-period.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-payroll-period-details',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatTableModule, MatIconModule],
  templateUrl: './payroll-period-details.component.html'
})
export class PayrollPeriodDetailsComponent implements OnInit {
  period: PayrollPeriodDetailsDto | null = null;
  transactionColumns = ['workerName', 'transactionType', 'amount', 'transactionDate', 'note'];
  typeLabels = SalaryTransactionTypeLabels;

  constructor(
    private service: PayrollPeriodService,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
  }

  private load(id: string): void {
    this.service.getById(id).subscribe({
      next: (res) => { this.period = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل بيانات الدورة'); }
    });
  }

  onClose(): void {
    if (!confirm('هل أنت متأكد من إغلاق هذه الدورة؟ لن تتمكن من التراجع.')) return;

    this.service.close(this.period!.id).subscribe({
      next: () => {
        this.alertService.showSuccess('تم إغلاق الدورة بنجاح');
        this.load(this.period!.id);
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'حدث خطأ أثناء إغلاق الدورة';
        this.alertService.showError(msg);
      }
    });
  }

  onBack(): void {
    this.router.navigate(['/hr/payroll-periods']);
  }
}
