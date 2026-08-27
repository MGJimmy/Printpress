import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CashTransactionService } from '../../services/cash-transaction.service';
import { CashAccountService } from '../../services/cash-account.service';
import { AlertService } from '../../../../core/services/alert.service';
import { CashAccountDto } from '../../models/cash-account.dto';

export interface TransferCashDialogData {
  fromCashAccountId: string;
}

@Component({
  selector: 'app-transfer-cash-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule,
  ],
  templateUrl: './transfer-cash-dialog.component.html',
})
export class TransferCashDialogComponent implements OnInit {
  form: FormGroup<{
    fromCashAccountId: FormControl<string>;
    toCashAccountId: FormControl<string>;
    amount: FormControl<number>;
    description: FormControl<string>;
    transactionDate: FormControl<Date | null>;
  }>;

  accounts: CashAccountDto[] = [];
  isSubmitting = false;

  constructor(
    private fb: NonNullableFormBuilder,
    public dialogRef: MatDialogRef<TransferCashDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TransferCashDialogData,
    private cashTransactionService: CashTransactionService,
    private cashAccountService: CashAccountService,
    private alertService: AlertService,
  ) {
    this.form = this.fb.group({
      fromCashAccountId: this.fb.control(data.fromCashAccountId || '', Validators.required),
      toCashAccountId: this.fb.control('', Validators.required),
      amount: this.fb.control(0, [Validators.required, Validators.min(0.01)]),
      description: this.fb.control(''),
      transactionDate: this.fb.control<Date | null>(new Date(), Validators.required),
    });
  }

  ngOnInit(): void {
    this.cashAccountService.getAll().subscribe({
      next: (response) => {
        this.accounts = response.data;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل الخزائن');
      }
    });
  }

  accountLabel(account: CashAccountDto): string {
    const type = account.type === 'SpareParts' ? 'قطع الغيار' : 'رئيسية';
    return `${account.name} (${type})`;
  }

  onConfirm(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { fromCashAccountId, toCashAccountId, amount, description, transactionDate } = this.form.getRawValue();
    if (fromCashAccountId === toCashAccountId) {
      this.alertService.showError('لا يمكن التحويل إلى نفس الخزينة');
      return;
    }

    this.isSubmitting = true;
    this.cashTransactionService.transfer({
      fromCashAccountId,
      toCashAccountId,
      amount,
      description,
      transactionDate: transactionDate ? (transactionDate as Date).toISOString() : new Date().toISOString(),
    }).subscribe({
      next: () => {
        this.alertService.showSuccess('تم التحويل بنجاح');
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSubmitting = false;
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
