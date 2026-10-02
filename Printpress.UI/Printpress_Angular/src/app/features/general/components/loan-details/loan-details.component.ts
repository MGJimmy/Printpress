import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, AbstractControl, FormControl, FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { LoanService } from '../../services/loan.service';
import { LoanDto } from '../../models/loan.dto';

@Component({
  selector: 'app-loan-details',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatDatepickerModule,
    MatNativeDateModule, MatTableModule, MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './loan-details.component.html',
  styleUrl: '../loans.shared.scss',
})
export class LoanDetailsComponent implements OnInit {
  loan: LoanDto | null = null;
  isLoading = false;
  isPaying = false;
  isVoiding = false;
  paymentColumns = ['transactionDate', 'amount', 'description', 'status', 'action'];
  payForm: FormGroup<{
    amount: FormControl<number | null>;
    occurredAt: FormControl<Date | null>;
    note: FormControl<string>;
  }>;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: NonNullableFormBuilder,
    private loanService: LoanService,
    private alertService: AlertService,
    private dialogService: DialogService,
  ) {
    this.payForm = this.fb.group({
      amount: this.fb.control<number | null>(null, [Validators.required, Validators.min(0.01), this.remainingValidator]),
      occurredAt: this.fb.control<Date | null>(new Date(), Validators.required),
      note: this.fb.control('', Validators.maxLength(500)),
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.goBack();
      return;
    }
    this.load(id);
  }

  get canVoidLoan(): boolean {
    return !!this.loan && !this.loan.isVoided && this.loan.paidAmount === 0;
  }

  get canPay(): boolean {
    return !!this.loan && !this.loan.isVoided && this.loan.remaining > 0;
  }

  goBack(): void {
    this.router.navigate(['/general/loans']);
  }

  private remainingValidator = (control: AbstractControl) => {
    const remaining = this.loan?.remaining ?? 0;
    const value = Number(control.value);
    if (!control.value || Number.isNaN(value) || value <= remaining) return null;
    return { exceedsRemaining: true };
  };

  private load(id: string): void {
    this.isLoading = true;
    this.loanService.getById(id).pipe(finalize(() => { this.isLoading = false; })).subscribe({
      next: (res) => {
        this.loan = { ...res.data, payments: res.data.payments ?? [] };
        this.payForm.controls.amount.updateValueAndValidity();
      },
      error: () => this.alertService.showError('حدث خطأ أثناء تحميل القرض'),
    });
  }

  private toIsoDate(value: Date): string {
    const year = value.getFullYear();
    const month = String(value.getMonth() + 1).padStart(2, '0');
    const day = String(value.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  pay(): void {
    if (!this.loan || !this.canPay) return;
    if (this.payForm.invalid || this.isPaying) {
      this.payForm.markAllAsTouched();
      this.alertService.showError('أكمل بيانات السداد المطلوبة');
      return;
    }
    const v = this.payForm.getRawValue();
    this.isPaying = true;
    this.loanService.pay(this.loan.id, {
      amount: v.amount!,
      occurredAt: v.occurredAt ? this.toIsoDate(v.occurredAt) : undefined,
      note: v.note,
    }).pipe(finalize(() => { this.isPaying = false; })).subscribe({
      next: () => {
        this.alertService.showSuccess('تم تسجيل السداد');
        this.payForm.patchValue({ amount: null, note: '' });
        this.payForm.markAsUntouched();
        this.load(this.loan!.id);
      },
    });
  }

  voidLoan(): void {
    if (!this.loan || !this.canVoidLoan) return;
    this.dialogService.promptDialog({
      title: 'إلغاء القرض',
      message: 'أدخل سبب الإلغاء للمتابعة.',
      fieldLabel: 'سبب الإلغاء',
      confirmText: 'تأكيد',
      cancelText: 'إلغاء',
      maxLength: 500,
    }).subscribe((reason) => {
      if (!reason || this.isVoiding) return;
      this.isVoiding = true;
      this.loanService.void(this.loan!.id, reason).pipe(finalize(() => { this.isVoiding = false; })).subscribe({
        next: () => {
          this.alertService.showSuccess('تم إلغاء القرض');
          this.load(this.loan!.id);
        },
      });
    });
  }

  voidPayment(paymentId: string, isVoided: boolean): void {
    if (!this.loan || isVoided) return;
    this.dialogService.promptDialog({
      title: 'إلغاء دفعة',
      message: 'أدخل سبب إلغاء الدفعة.',
      fieldLabel: 'سبب الإلغاء',
      confirmText: 'تأكيد',
      cancelText: 'إلغاء',
      maxLength: 500,
    }).subscribe((reason) => {
      if (!reason || this.isVoiding) return;
      this.isVoiding = true;
      this.loanService.voidPayment(this.loan!.id, paymentId, reason).pipe(
        finalize(() => { this.isVoiding = false; })
      ).subscribe({
        next: () => {
          this.alertService.showSuccess('تم إلغاء الدفعة');
          this.load(this.loan!.id);
        },
      });
    });
  }
}
