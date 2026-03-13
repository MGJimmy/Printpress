import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { Subscription } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { WorkerService } from '../../services/worker.service';
import { WorkerCreateDto, WorkerUpdateDto } from '../../models/worker.dto';

@Component({
  selector: 'app-worker-upsert',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
    MatSelectModule
  ],
  templateUrl: './worker-upsert.component.html'
})
export class WorkerUpsertComponent implements OnInit, OnDestroy {
  isEditMode = false;
  workerId: string | null = null;
  showMonthlySalary = false;
  showDailySalary = false;

  form: FormGroup<{
    name: FormControl<string>;
    phoneNumber: FormControl<string>;
    address: FormControl<string>;
    notes: FormControl<string>;
    salaryType: FormControl<number>;
    monthlySalary: FormControl<number | null>;
    dailySalary: FormControl<number | null>;
  }>;

  private subs = new Subscription();

  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private workerService: WorkerService
  ) {
    this.form = this.fb.group({
      name: this.fb.control('', Validators.required),
      phoneNumber: this.fb.control(''),
      address: this.fb.control(''),
      notes: this.fb.control(''),
      salaryType: this.fb.control(1, Validators.required),
      monthlySalary: new FormControl<number | null>(null),
      dailySalary: new FormControl<number | null>(null)
    });
  }

  ngOnInit(): void {
    this.workerId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.workerId;

    this.subs.add(
      this.form.controls.salaryType.valueChanges.subscribe(val => {
        this.updateSalaryVisibility(val);
      })
    );
    this.updateSalaryVisibility(1);

    if (this.isEditMode) {
      this.workerService.getById(this.workerId!).subscribe({
        next: (res) => {
          const d = res.data;
          this.form.patchValue({
            name: d.name,
            phoneNumber: d.phoneNumber,
            address: d.address,
            notes: d.notes,
            salaryType: d.salaryType,
            monthlySalary: d.monthlySalary ?? null,
            dailySalary: d.dailySalary ?? null
          });
          this.updateSalaryVisibility(d.salaryType);
        },
        error: () => { this.alertService.showError('حدث خطأ أثناء تحميل بيانات العامل'); }
      });
    }
  }

  private updateSalaryVisibility(salaryType: number): void {
    this.showMonthlySalary = salaryType === 1;
    this.showDailySalary = salaryType === 2;
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    if (this.isEditMode) {
      const payload: WorkerUpdateDto = {
        id: this.workerId!,
        name: raw.name,
        phoneNumber: raw.phoneNumber,
        address: raw.address,
        notes: raw.notes,
        salaryType: raw.salaryType,
        monthlySalary: raw.salaryType === 1 ? raw.monthlySalary ?? undefined : undefined,
        dailySalary: raw.salaryType === 2 ? raw.dailySalary ?? undefined : undefined
      };
      this.workerService.update(payload).subscribe({
        next: () => {
          this.alertService.showSuccess('تم تعديل بيانات العامل بنجاح');
          this.router.navigate(['/hr/workers']);
        },
        error: (err) => {
          const msg = err?.error?.message ?? 'حدث خطأ أثناء حفظ البيانات';
          this.alertService.showError(msg);
        }
      });
    } else {
      const payload: WorkerCreateDto = {
        name: raw.name,
        phoneNumber: raw.phoneNumber,
        address: raw.address,
        notes: raw.notes,
        salaryType: raw.salaryType,
        monthlySalary: raw.salaryType === 1 ? raw.monthlySalary ?? undefined : undefined,
        dailySalary: raw.salaryType === 2 ? raw.dailySalary ?? undefined : undefined
      };
      this.workerService.add(payload).subscribe({
        next: () => {
          this.alertService.showSuccess('تم إضافة العامل بنجاح');
          this.router.navigate(['/hr/workers']);
        },
        error: (err) => {
          const msg = err?.error?.message ?? 'حدث خطأ أثناء حفظ البيانات';
          this.alertService.showError(msg);
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/hr/workers']);
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}
