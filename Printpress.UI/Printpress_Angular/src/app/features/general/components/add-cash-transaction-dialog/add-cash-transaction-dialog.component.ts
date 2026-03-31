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
import { AlertService } from '../../../../core/services/alert.service';
import { ExternalOrderDto } from '../../models/cash-transaction.dto';
import { SearchSelectComponent, SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';

export interface AddCashTransactionDialogData {
  cashAccountId: string;
}

const IN_CATEGORIES = [
  { value: 'CapitalInjection', label: 'ضخ رأس المال' },
  { value: 'Other', label: 'أخرى' },
];

const OUT_CATEGORIES = [
  { value: 'Expenses', label: 'مصروفات' },
  { value: 'Maintenance', label: 'صيانة' },
  { value: 'ExternalServices', label: 'خدمات خارجية' },
  { value: 'Other', label: 'أخرى' },
];

@Component({
  selector: 'app-add-cash-transaction-dialog',
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
    SearchSelectComponent,
  ],
  templateUrl: './add-cash-transaction-dialog.component.html',
})
export class AddCashTransactionDialogComponent implements OnInit {
  form: FormGroup<{
    type: FormControl<string>;
    category: FormControl<string>;
    amount: FormControl<number>;
    description: FormControl<string>;
    transactionDate: FormControl<Date | null>;
    groupId: FormControl<string>;
  }>;

  transactionTypeOptions = [
    { value: 'In', label: 'وارد' },
    { value: 'Out', label: 'صادر' },
  ];

  filteredCategories: { value: string; label: string }[] = [];
  externalOrders: ExternalOrderDto[] = [];
  externalOrderItems: SearchSelectItem[] = [];
  showGroupSelector = false;
  isSubmitting = false;

  constructor(
    private fb: NonNullableFormBuilder,
    public dialogRef: MatDialogRef<AddCashTransactionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AddCashTransactionDialogData,
    private cashTransactionService: CashTransactionService,
    private alertService: AlertService,
  ) {
    this.form = this.fb.group({
      type: this.fb.control('', Validators.required),
      category: this.fb.control('', Validators.required),
      amount: this.fb.control(0, [Validators.required, Validators.min(0.01)]),
      description: this.fb.control(''),
      transactionDate: this.fb.control<Date | null>(new Date(), Validators.required),
      groupId: this.fb.control(''),
    });
  }

  ngOnInit(): void {
    this.form.controls.type.valueChanges.subscribe((type) => {
      this.filteredCategories = type === 'In' ? IN_CATEGORIES : OUT_CATEGORIES;
      this.form.controls.category.setValue('');
      this.showGroupSelector = false;
      this.form.controls.groupId.setValue('');
    });

    this.form.controls.category.valueChanges.subscribe((category) => {
      this.showGroupSelector = category === 'ExternalServices';
      if (this.showGroupSelector && this.externalOrders.length === 0) {
        this.loadExternalOrders();
      }
      if (!this.showGroupSelector) {
        this.form.controls.groupId.setValue('');
      }
    });
  }

  private loadExternalOrders(): void {
    this.cashTransactionService.getExternalOrders().subscribe({
      next: (response) => {
        this.externalOrders = response.data;
        this.externalOrderItems = this.externalOrders.map(o => ({
          id: o.orderId,
          name: o.orderName
        }));
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل الطلبات الخارجية');
      }
    });
  }

  onConfirm(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.showGroupSelector && !this.form.controls.groupId.value) {
      this.alertService.showError('يجب اختيار مجموعة الطلب للخدمات الخارجية');
      return;
    }

    const { type, category, amount, description, transactionDate, groupId } = this.form.getRawValue();

    const payload: any = {
      cashAccountId: this.data.cashAccountId,
      type,
      category,
      amount,
      description,
      transactionDate: transactionDate ? (transactionDate as Date).toISOString() : new Date().toISOString(),
    };

    if (this.showGroupSelector && groupId) {
      payload.referenceId = groupId;
      payload.referenceType = 'Order';
    }

    this.isSubmitting = true;
    this.cashTransactionService.add(payload).subscribe({
      next: () => {
        this.alertService.showSuccess('تم إضافة الحركة بنجاح');
        this.dialogRef.close(true);
      },
      error: (err: any) => {
        const msg = err?.error?.errors?.[0] || 'حدث خطأ أثناء إضافة الحركة';
        this.alertService.showError(msg);
        this.isSubmitting = false;
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
