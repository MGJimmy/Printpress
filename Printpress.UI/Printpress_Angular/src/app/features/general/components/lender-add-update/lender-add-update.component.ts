import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { LenderService } from '../../services/loan.service';

@Component({
  selector: 'app-lender-add-update',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule,
  ],
  templateUrl: './lender-add-update.component.html',
  styleUrl: '../loans.shared.scss',
})
export class LenderAddUpdateComponent implements OnInit {
  id: string | null = null;
  isSaving = false;
  form: FormGroup<{
    name: FormControl<string>;
    phone: FormControl<string>;
    notes: FormControl<string>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private lenderService: LenderService,
    private alertService: AlertService,
  ) {
    this.form = this.fb.group({
      name: this.fb.control('', [Validators.required, Validators.maxLength(200)]),
      phone: this.fb.control('', Validators.maxLength(50)),
      notes: this.fb.control('', Validators.maxLength(500)),
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (!this.id) return;
    this.lenderService.getById(this.id).subscribe({
      next: (res) => this.form.patchValue({
        name: res.data.name ?? '',
        phone: res.data.phone ?? '',
        notes: res.data.notes ?? '',
      }),
      error: () => this.alertService.showError('حدث خطأ أثناء تحميل المقرض'),
    });
  }

  get isEdit(): boolean {
    return !!this.id;
  }

  goBack(): void {
    this.router.navigate(['/general/lenders']);
  }

  save(): void {
    if (this.form.invalid || this.isSaving) {
      this.form.markAllAsTouched();
      this.alertService.showError('أكمل الحقول المطلوبة قبل الحفظ');
      return;
    }
    const payload = this.form.getRawValue();
    this.isSaving = true;
    const req = this.isEdit
      ? this.lenderService.update(this.id!, payload)
      : this.lenderService.add(payload);
    req.pipe(finalize(() => { this.isSaving = false; })).subscribe({
      next: () => {
        this.alertService.showSuccess(this.isEdit ? 'تم تعديل المقرض' : 'تم إضافة المقرض');
        this.goBack();
      },
    });
  }
}
