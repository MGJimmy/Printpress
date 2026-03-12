import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { PayrollPeriodService } from '../../services/payroll-period.service';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-payroll-period-add',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule
  ],
  templateUrl: './payroll-period-add.component.html'
})
export class PayrollPeriodAddComponent {
  form: FormGroup<{
    name: FormControl<string>;
    startDate: FormControl<Date | null>;
    endDate: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private service: PayrollPeriodService,
    private alertService: AlertService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: this.fb.control('', Validators.required),
      startDate: new FormControl<Date | null>(null, Validators.required),
      endDate: new FormControl<Date | null>(null, Validators.required)
    });
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.service.add({
      name: raw.name,
      startDate: raw.startDate!.toISOString(),
      endDate: raw.endDate!.toISOString()
    }).subscribe({
      next: () => {
        this.alertService.showSuccess('تم إضافة دورة الرواتب بنجاح');
        this.router.navigate(['/hr/payroll-periods']);
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'حدث خطأ أثناء حفظ البيانات';
        this.alertService.showError(msg);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/hr/payroll-periods']);
  }
}
