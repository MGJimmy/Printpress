import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PayrollPeriodService } from '../../services/payroll-period.service';
import { PayrollPeriodDetailsDto, SalaryTransactionTypeLabels } from '../../models/payroll-period.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-payroll-period-details',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, TableTemplateComponent],
  templateUrl: './payroll-period-details.component.html'
})
export class PayrollPeriodDetailsComponent implements OnInit {
  period: PayrollPeriodDetailsDto | null = null;
  flatTransactions: any[] = [];
  typeLabels = SalaryTransactionTypeLabels;

  transactionColDefs: TableColDefinitionModel[] = [
    { column: 'workerName', headerName: 'اسم العامل' },
    { column: 'transactionType', headerName: 'نوع الحركة' },
    { column: 'amount', headerName: 'المبلغ' },
    { column: 'transactionDate', headerName: 'التاريخ' },
    { column: 'note', headerName: 'ملاحظة' }
  ];

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
      next: (res) => {
        this.period = res.data;
        this.flatTransactions = res.data.transactions.map(t => ({
          workerName: t.workerName,
          transactionType: this.typeLabels[t.transactionType] ?? t.transactionType,
          amount: t.amount,
          transactionDate: t.transactionDate ? new Date(t.transactionDate).toLocaleDateString('ar-EG') : '—',
          note: t.note || '—'
        }));
      },
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
