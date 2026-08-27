import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { AlertService } from '../../../../core/services/alert.service';
import { CashAccountService } from '../../services/cash-account.service';
import { CashTransactionService } from '../../services/cash-transaction.service';
import { CashAccountDto } from '../../models/cash-account.dto';
import { CashTransactionDto } from '../../models/cash-transaction.dto';
import { AddCashTransactionDialogComponent } from '../add-cash-transaction-dialog/add-cash-transaction-dialog.component';

@Component({
  selector: 'app-cash-account-view',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatDialogModule,
    TableTemplateComponent,
  ],
  templateUrl: './cash-account-view.component.html',
})
export class CashAccountViewComponent implements OnInit {
  account: CashAccountDto | null = null;

  form: FormGroup<{
    name: FormControl<string>;
    type: FormControl<string>;
    balance: FormControl<number>;
  }>;

  transactions: CashTransactionDto[] = [];
  totalTransactionsCount = 0;
  currentPage = 1;
  pageSize = 10;

  filterDateFrom: Date | null = null;
  filterDateTo: Date | null = null;
  filterType: string = '';
  filterCategory: string = '';

  transactionTypeOptions = [
    { value: '', label: 'الكل' },
    { value: 'In', label: 'وارد' },
    { value: 'Out', label: 'صادر' },
  ];

  categoryOptions = [
    { value: '', label: 'الكل' },
    { value: 'Sales', label: 'مبيعات' },
    { value: 'SalesReturn', label: 'مرتجع مبيعات' },
    { value: 'Purchases', label: 'مشتريات' },
    { value: 'Expenses', label: 'مصروفات' },
    { value: 'CapitalInjection', label: 'ضخ رأس المال' },
    { value: 'Maintenance', label: 'صيانة' },
    { value: 'ExternalServices', label: 'خدمات خارجية' },
    { value: 'Salaries', label: 'رواتب' },
    { value: 'Other', label: 'أخرى' },
  ];

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'نوع الحركة', column: 'type' },
    { headerName: 'الفئة', column: 'category' },
    { headerName: 'المبلغ', column: 'amount' },
    { headerName: 'الوصف', column: 'description' },
    { headerName: 'تاريخ الحركة', column: 'transactionDate' },
  ];

  private accountId!: string;

  constructor(
    private fb: NonNullableFormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private cashAccountService: CashAccountService,
    private cashTransactionService: CashTransactionService,
    private dialog: MatDialog,
  ) {
    this.form = this.fb.group({
      name: this.fb.control({ value: '', disabled: true }),
      type: this.fb.control({ value: '', disabled: true }),
      balance: this.fb.control({ value: 0, disabled: true }),
    });
  }

  ngOnInit(): void {
    this.accountId = this.activatedRoute.snapshot.paramMap.get('id')!;
    this.loadAccount();
    this.loadTransactions();
  }

  private loadAccount(): void {
    this.cashAccountService.getById(this.accountId).subscribe({
      next: (response) => {
        this.account = response.data;
        this.form.patchValue({
          name: this.account.name,
          type: this.account.type === 'SpareParts' ? 'قطع الغيار' : 'رئيسية',
          balance: this.account.balance,
        });
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل بيانات الخزينة');
      }
    });
  }

  private loadTransactions(): void {
    const dateFrom = this.filterDateFrom ? this.filterDateFrom.toISOString() : undefined;
    const dateTo = this.filterDateTo ? this.filterDateTo.toISOString() : undefined;
    const type = this.filterType || undefined;
    const category = this.filterCategory || undefined;

    this.cashTransactionService.getByCashAccountId(
      this.accountId, this.currentPage, this.pageSize, dateFrom, dateTo, type, category
    ).subscribe({
      next: (response) => {
        this.transactions = (response.data.items as CashTransactionDto[]).map((t) => ({
          ...t,
          type: this.toTypeLabel(t.type),
          category: this.toCategoryLabel(t.category),
        }));
        this.totalTransactionsCount = response.data.totalCount;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل الحركات');
      }
    });
  }

  applyFilter(): void {
    this.currentPage = 1;
    this.loadTransactions();
  }

  resetFilter(): void {
    this.filterDateFrom = null;
    this.filterDateTo = null;
    this.filterType = '';
    this.filterCategory = '';
    this.currentPage = 1;
    this.loadTransactions();
  }

  onPageChanged(event: PageChangedModel): void {
    this.currentPage = event.currentPage;
    this.pageSize = event.pageSize;
    this.loadTransactions();
  }

  onAddTransaction(): void {
    const dialogRef = this.dialog.open(AddCashTransactionDialogComponent, {
      width: '560px',
      data: { cashAccountId: this.accountId },
    });

    dialogRef.afterClosed().subscribe((added: boolean) => {
      if (added) {
        this.loadAccount();
        this.loadTransactions();
      }
    });
  }

  onBack(): void {
    this.router.navigate(['/general/cash-accounts']);
  }

  private toTypeLabel(type: string): string {
    return type === 'In' ? 'وارد' : type === 'Out' ? 'صادر' : type;
  }

  private toCategoryLabel(category: string): string {
    return this.categoryOptions.find((c) => c.value === category)?.label ?? category;
  }
}
