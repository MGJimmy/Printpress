import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormGroup, NonNullableFormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { AlertService } from '../../../../core/services/alert.service';
import { LenderService, LoanService } from '../../services/loan.service';
import { LenderDto, LoanListDto } from '../../models/loan.dto';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-loan-list',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule, TableTemplateComponent,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './loan-list.component.html',
  styleUrl: '../loans.shared.scss',
})
export class LoanListComponent implements OnInit {
  loans: Array<LoanListDto & { statusLabel: string }> = [];
  lenders: LenderDto[] = [];
  totalCount = 0;
  pageNumber = DEFAULT_PAGE_NUMBER;
  pageSize = DEFAULT_PAGE_SIZE;

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'رقم القرض', column: 'loanNumber' },
    { headerName: 'المقرض', column: 'lenderName' },
    { headerName: 'الأصل', column: 'principal' },
    { headerName: 'المسدد', column: 'paidAmount' },
    { headerName: 'المتبقي', column: 'remaining' },
    { headerName: 'التاريخ', column: 'occurredAt' },
    { headerName: 'الحالة', column: 'statusLabel' },
  ];

  filterForm: FormGroup<{
    lenderId: FormControl<string>;
    hasRemaining: FormControl<string>;
    isVoided: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private loanService: LoanService,
    private lenderService: LenderService,
    private alertService: AlertService,
    private router: Router,
  ) {
    this.filterForm = this.fb.group({
      lenderId: this.fb.control(''),
      hasRemaining: this.fb.control(''),
      isVoided: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
    });
  }

  ngOnInit(): void {
    this.lenderService.getAll().subscribe({ next: (res) => this.lenders = res.data ?? [] });
    this.load();
  }

  private toDateParam(value: Date | null): string | undefined {
    if (!value) return undefined;
    const year = value.getFullYear();
    const month = String(value.getMonth() + 1).padStart(2, '0');
    const day = String(value.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  load(): void {
    const v = this.filterForm.getRawValue();
    this.loanService.getAll(this.pageNumber, this.pageSize, {
      lenderId: v.lenderId || undefined,
      hasRemaining: v.hasRemaining === '' ? null : v.hasRemaining === 'true',
      isVoided: v.isVoided === '' ? null : v.isVoided === 'true',
      dateFrom: this.toDateParam(v.dateFrom),
      dateTo: this.toDateParam(v.dateTo),
    }).subscribe({
      next: (res) => {
        this.loans = (res.data.items ?? []).map((l) => ({
          ...l,
          occurredAt: l.occurredAt?.slice(0, 10),
          statusLabel: l.isVoided ? 'ملغى' : (l.isClosed ? 'مسدد' : 'ساري'),
        }));
        this.totalCount = res.data.totalCount;
      },
      error: () => this.alertService.showError('حدث خطأ أثناء تحميل القروض'),
    });
  }

  onSearch(): void {
    this.pageNumber = DEFAULT_PAGE_NUMBER;
    this.load();
  }

  onPageChange(event: PageChangedModel): void {
    this.pageNumber = event.currentPage;
    this.pageSize = event.pageSize;
    this.load();
  }

  onAdd(): void {
    this.router.navigate(['/general/loans/new']);
  }

  onView(id: string): void {
    this.router.navigate(['/general/loans', id]);
  }
}
