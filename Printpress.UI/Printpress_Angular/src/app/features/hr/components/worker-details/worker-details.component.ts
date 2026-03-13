import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormControl, FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { WorkerService } from '../../services/worker.service';
import { WorkerSalaryTransactionService } from '../../services/worker-salary-transaction.service';
import { PayrollPeriodService } from '../../services/payroll-period.service';
import {
  WorkerDetailsDto,
  WorkerSalaryTransactionDto,
  WorkerProductionDto,
  AddSalaryTransactionDto,
  SalaryTypeLabels,
  SalaryTransactionTypeLabels
} from '../../models/worker.dto';
import { PayrollPeriodDto } from '../../models/payroll-period.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-worker-details',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    FormsModule
  ],
  templateUrl: './worker-details.component.html'
})
export class WorkerDetailsComponent implements OnInit {
  worker: WorkerDetailsDto | null = null;
  openPeriods: PayrollPeriodDto[] = [];

  salaryTypeLabels = SalaryTypeLabels;
  transactionTypeLabels = SalaryTransactionTypeLabels;

  transactionColumns = ['payrollPeriodName', 'transactionType', 'amount', 'transactionDate', 'note', 'actions'];
  productionColumns = ['productionDate', 'serviceCategoryName', 'orderName', 'quantity', 'notes'];

  productionDateFrom: Date | null = null;
  productionDateTo: Date | null = null;

  transactionForm: FormGroup<{
    payrollPeriodId: FormControl<string>;
    transactionType: FormControl<number>;
    amount: FormControl<number | null>;
    transactionDate: FormControl<Date | null>;
    note: FormControl<string>;
  }>;

  transactionTypeOptions = [
    { value: 1, label: 'سلفة' },
    { value: 2, label: 'دفعة يومية' },
    { value: 3, label: 'راتب شهري' },
    { value: 4, label: 'مكافأة' },
    { value: 5, label: 'خصم / غرامة' },
    { value: 6, label: 'تسوية' }
  ];

  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private workerService: WorkerService,
    private transactionService: WorkerSalaryTransactionService,
    private payrollPeriodService: PayrollPeriodService,
    private alertService: AlertService
  ) {
    this.transactionForm = this.fb.group({
      payrollPeriodId: this.fb.control('', Validators.required),
      transactionType: this.fb.control(1, Validators.required),
      amount: new FormControl<number | null>(null, Validators.required),
      transactionDate: new FormControl<Date | null>(new Date(), Validators.required),
      note: this.fb.control('')
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadWorker(id);
    this.loadOpenPeriods();
  }

  private loadWorker(id: string, productionDateFrom?: Date | null, productionDateTo?: Date | null): void {
    const fromStr = productionDateFrom ? productionDateFrom.toISOString() : undefined;
    const toStr = productionDateTo ? productionDateTo.toISOString() : undefined;

    this.workerService.getById(id, fromStr, toStr).subscribe({
      next: (res) => { this.worker = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل بيانات العامل'); }
    });
  }

  private loadOpenPeriods(): void {
    this.payrollPeriodService.getAll().subscribe({
      next: (res) => {
        this.openPeriods = res.data.filter(p => !p.isClosed);
        this.preselectTodayPeriod();
      },
      error: () => {}
    });
  }

  private preselectTodayPeriod(): void {
    const today = new Date().toISOString().split('T')[0];
    const todayPeriod = this.openPeriods.find(p =>
      p.startDate.split('T')[0] <= today && p.endDate.split('T')[0] >= today
    );
    if (todayPeriod) {
      this.transactionForm.controls.payrollPeriodId.setValue(todayPeriod.id);
    }
  }

  onFilterProductions(): void {
    if (!this.worker) return;
    this.loadWorker(this.worker.id, this.productionDateFrom, this.productionDateTo);
  }

  onAddTransaction(): void {
    if (this.transactionForm.invalid) {
      this.transactionForm.markAllAsTouched();
      return;
    }

    const raw = this.transactionForm.getRawValue();
    const payload: AddSalaryTransactionDto = {
      workerId: this.worker!.id,
      payrollPeriodId: raw.payrollPeriodId,
      transactionType: raw.transactionType,
      amount: raw.amount!,
      transactionDate: raw.transactionDate!.toISOString(),
      note: raw.note
    };

    this.transactionService.add(payload).subscribe({
      next: () => {
        this.alertService.showSuccess('تم إضافة الحركة المالية بنجاح');
        this.transactionForm.controls.amount.reset();
        this.transactionForm.controls.note.reset();
        this.loadWorker(this.worker!.id, this.productionDateFrom, this.productionDateTo);
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'حدث خطأ أثناء إضافة الحركة';
        this.alertService.showError(msg);
      }
    });
  }

  onDeleteTransaction(transactionId: string): void {
    if (!confirm('هل أنت متأكد من حذف هذه الحركة؟')) return;

    this.transactionService.delete(transactionId).subscribe({
      next: () => {
        this.alertService.showSuccess('تم حذف الحركة بنجاح');
        this.loadWorker(this.worker!.id, this.productionDateFrom, this.productionDateTo);
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'حدث خطأ أثناء حذف الحركة';
        this.alertService.showError(msg);
      }
    });
  }

  onBack(): void {
    this.router.navigate(['/hr/workers']);
  }

  isMonthly(): boolean {
    return this.worker?.salaryType === 1;
  }
}
