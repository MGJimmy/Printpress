import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder, AbstractControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { UserService } from '../../services/user.service';
import { AlertService } from '../../../../core/services/alert.service';

function passwordsMatch(ctrl: AbstractControl): ValidationErrors | null {
  const password = ctrl.get('newPassword')?.value;
  const confirm = ctrl.get('confirmPassword')?.value;
  return password && confirm && password !== confirm ? { mismatch: true } : null;
}

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './change-password.component.html'
})
export class ChangePasswordComponent implements OnInit {
  userId!: string;
  isSaving = false;

  form: FormGroup<{
    newPassword: FormControl<string>;
    confirmPassword: FormControl<string>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private alertService: AlertService
  ) {
    this.form = this.fb.group({
      newPassword: this.fb.control('', [Validators.required, Validators.minLength(7)]),
      confirmPassword: this.fb.control('', Validators.required)
    }, { validators: passwordsMatch });
  }

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id')!;
  }

  onSave(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSaving = true;
    const { newPassword } = this.form.getRawValue();

    this.userService.changePassword(this.userId, newPassword).subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res?.success === false) { this.alertService.showError(res.errorMessage ?? 'حدث خطأ'); return; }
        this.alertService.showSuccess('تم تغيير كلمة المرور بنجاح');
        this.router.navigate(['/users']);
      },
      error: () => { this.isSaving = false; this.alertService.showError('حدث خطأ أثناء تغيير كلمة المرور'); }
    });
  }

  onBack(): void {
    this.router.navigate(['/users']);
  }
}
