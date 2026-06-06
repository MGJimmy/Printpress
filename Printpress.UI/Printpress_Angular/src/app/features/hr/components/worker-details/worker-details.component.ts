import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormControl, FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { WorkerService } from '../../services/worker.service';
import { WorkerSalaryTransactionService } from '../../services/worker-salary-transaction.service';
import { PayrollPeriodService } from '../../services/payroll-period.service';
import { InventoryTransactionService } from '../../../inventory/services/inventory-transaction.service';
import { InventoryService } from '../../../inventory/services/inventory.service';
import { InventoryServicesUsageReportService } from '../../../reports/services/inventory-services-usage-report.service';
import { InventoryCategoryFilterDto } from '../../../reports/models/order-inventory-items-report.dto';
import { InventoryItemDto } from '../../../inventory/models/inventory-item.dto';
import {
  WorkerDetailsDto,
  AddSalaryTransactionDto,
  SalaryTypeLabels,
  SalaryTransactionTypeLabels
} from '../../models/worker.dto';
import { PayrollPeriodDto } from '../../models/payroll-period.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { MatTableModule } from '@angular/material/table';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { DatePipe } from '@angular/common';
import { TransactionType } from '../../models/transaction-type.enum';
import { SalaryType } from '../../models/salary-type.enum';

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
    FormsModule,
    TableTemplateComponent,
    SharedPaginationComponent,
    DatePipe
  ],
  templateUrl: './worker-details.component.html'
})
export class WorkerDetailsComponent implements OnInit {
  worker: WorkerDetailsDto | null = null;
  openPeriods: PayrollPeriodDto[] = [];
  flatTransactions: any[] = [];
  flatProductions: any[] = [];

  workerId: string = '';

  inventoryTransactions: any[] = [];
  invTotalCount = 0;
  invPageSize = 10;
  invPageNumber = 1;
  invCategoryId: number | null = null;
  invItemId: string | null = null;
  invDateFrom: Date | null = null;
  invDateTo: Date | null = null;
  invCategories: InventoryCategoryFilterDto[] = [];
  invItems: InventoryItemDto[] = [];

  salaryTypeLabels = SalaryTypeLabels;
  transactionTypeLabels = SalaryTransactionTypeLabels;

  transactionColDefs: TableColDefinitionModel[] = [
    { column: 'payrollPeriodName', headerName: 'دورة الراتب' },
    { column: 'transactionType', headerName: 'نوع الحركة' },
    { column: 'amount', headerName: 'المبلغ' },
    { column: 'transactionDate', headerName: 'التاريخ' },
    { column: 'note', headerName: 'ملاحظة' }
  ];

  productionColDefs: TableColDefinitionModel[] = [
    { column: 'productionDate', headerName: 'التاريخ' },
    { column: 'serviceCategoryName', headerName: 'فئة الخدمة' },
    { column: 'orderName', headerName: 'المجموعة / الطلبية' },
    { column: 'quantity', headerName: 'الكمية' },
    { column: 'notes', headerName: 'ملاحظات' }
  ];

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
    { value: TransactionType.Advance, label: 'سلفة' },
    { value: TransactionType.Salary, label: 'راتب' },
    { value: TransactionType.Bonus, label: 'مكافأة' },
    { value: TransactionType.Penalty, label: 'خصم / غرامة' },
    { value: TransactionType.Settlement, label: 'تسوية' },
    { value: TransactionType.SalaryAdvancePayment, label: 'رد سلفة' }
  ];


  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private workerService: WorkerService,
    private transactionService: WorkerSalaryTransactionService,
    private payrollPeriodService: PayrollPeriodService,
    private alertService: AlertService,
    private invTransactionService: InventoryTransactionService,
    private inventoryService: InventoryService,
    private reportService: InventoryServicesUsageReportService
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
    this.workerId = id;
    this.loadWorker(id);
    this.loadOpenPeriods();
    this.loadInvCategories();
    this.loadInventoryTransactions();
  }

  private loadWorker(id: string, productionDateFrom?: Date | null, productionDateTo?: Date | null): void {
    const fromStr = productionDateFrom ? productionDateFrom.toISOString() : undefined;
    const toStr = productionDateTo ? productionDateTo.toISOString() : undefined;

    this.workerService.getById(id, fromStr, toStr).subscribe({
      next: (res) => {
        this.worker = res.data;
        this.flatTransactions = res.data.transactions.map(t => ({
          id: t.id,
          payrollPeriodName: t.payrollPeriodName,
          transactionType: this.transactionTypeLabels[t.transactionType] ?? t.transactionType,
          amount: t.amount,
          transactionDate: t.transactionDate ? new Date(t.transactionDate).toLocaleDateString('ar-EG') : '—',
          note: t.note || '—'
        }));
        this.flatProductions = res.data.productions.map(p => ({
          productionDate: p.productionDate ? new Date(p.productionDate).toLocaleDateString('ar-EG') : '—',
          serviceCategoryName: p.serviceCategoryName,
          orderName: p.orderName,
          quantity: p.quantity,
          notes: p.notes || '—'
        }));
      },
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

  resetProductionFilter(): void {
    this.productionDateFrom = null;
    this.productionDateTo = null;

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

  loadInvCategories(): void {
    this.reportService.getInventoryCategories().subscribe({
      next: (res) => { this.invCategories = res.data; }
    });
  }

  onInvCategoryChange(): void {
    this.invItemId = null;
    this.invItems = [];
    if (this.invCategoryId) {
      this.inventoryService.getByCategory(this.invCategoryId).subscribe({
        next: (res) => { this.invItems = res.data; }
      });
    }
  }

  loadInventoryTransactions(): void {
    const dateFrom = this.invDateFrom ? this.invDateFrom.toISOString() : undefined;
    const dateTo = this.invDateTo ? this.invDateTo.toISOString() : undefined;
    this.invTransactionService.getByWorkerId(
      this.workerId,
      this.invPageNumber,
      this.invPageSize,
      this.invCategoryId ?? undefined,
      this.invItemId ?? undefined,
      dateFrom,
      dateTo
    ).subscribe({
      next: (res: any) => {
        this.inventoryTransactions = res.data?.items ?? res.data ?? [];
        this.invTotalCount = res.data?.totalCount ?? 0;
      }
    });
  }

  onInvPageChange(event: any): void {
    this.invPageNumber = event.pageIndex + 1;
    this.invPageSize = event.pageSize;
    this.loadInventoryTransactions();
  }
  

  applyInvFilter(): void {
    this.invPageNumber = 1;
    this.loadInventoryTransactions();
  }

  resetInvFilter(): void {
    this.invCategoryId = null;
    this.invItemId = null;
    this.invItems = [];
    this.invDateFrom = null;
    this.invDateTo = null;
    this.invPageNumber = 1;
    this.loadInventoryTransactions();
  }

  onBack(): void {
    this.router.navigate(['/hr/workers']);
  }

  isMonthly(): boolean {
    return this.worker?.salaryType === SalaryType.Monthly;
  }
}
