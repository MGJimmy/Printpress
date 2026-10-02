import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { CashAccountService } from '../../services/cash-account.service';
import { LenderService, LoanService } from '../../services/loan.service';
import { CashAccountDto } from '../../models/cash-account.dto';
import { LenderDto } from '../../models/loan.dto';
import { SearchSelectComponent, SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';

@Component({
  selector: 'app-loan-add',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule,
    SearchSelectComponent,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './loan-add.component.html',
  styleUrl: '../loans.shared.scss',
})
export class LoanAddComponent implements OnInit {
  isSaving = false;
  lenders: SearchSelectItem[] = [];
  accounts: CashAccountDto[] = [];
  form: FormGroup<{
    lenderId: FormControl<string>;
    cashAccountId: FormControl<string>;
    principal: FormControl<number | null>;
    occurredAt: FormControl<Date | null>;
    notes: FormControl<string>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private router: Router,
    private loanService: LoanService,
    private lenderService: LenderService,
    private cashAccountService: CashAccountService,
    private alertService: AlertService,
  ) {
    this.form = this.fb.group({
      lenderId: this.fb.control('', Validators.required),
      cashAccountId: this.fb.control('', Validators.required),
      principal: this.fb.control<number | null>(null, [Validators.required, Validators.min(0.01)]),
      occurredAt: this.fb.control<Date | null>(new Date(), Validators.required),
      notes: this.fb.control('', Validators.maxLength(500)),
    });
  }

  ngOnInit(): void {
    this.lenderService.getAll().subscribe({
      next: (res) => this.lenders = (res.data ?? []).map((l: LenderDto) => ({ id: l.id, name: l.name })),
    });
    this.cashAccountService.getAll().subscribe({
      next: (res) => this.accounts = res.data ?? [],
    });
  }

  goBack(): void {
    this.router.navigate(['/general/loans']);
  }

  private toIsoDate(value: Date): string {
    const year = value.getFullYear();
    const month = String(value.getMonth() + 1).padStart(2, '0');
    const day = String(value.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  save(): void {
    if (this.form.invalid || this.isSaving) {
      this.form.markAllAsTouched();
      this.alertService.showError('أكمل الحقول المطلوبة قبل الحفظ');
      return;
    }
    const v = this.form.getRawValue();
    this.isSaving = true;
    this.loanService.add({
      lenderId: v.lenderId,
      principal: v.principal!,
      cashAccountId: v.cashAccountId,
      occurredAt: this.toIsoDate(v.occurredAt!),
      notes: v.notes,
    }).pipe(finalize(() => { this.isSaving = false; })).subscribe({
      next: (res) => {
        this.alertService.showSuccess('تم تسجيل القرض');
        this.router.navigate(['/general/loans', res.data.id]);
      },
    });
  }
}
