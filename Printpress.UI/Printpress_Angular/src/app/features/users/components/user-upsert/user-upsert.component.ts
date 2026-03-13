import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-user-upsert',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './user-upsert.component.html'
})
export class UserUpsertComponent implements OnInit {
  isEdit = false;
  userId: string | null = null;
  isSaving = false;
  availableRoles: string[] = [];

  form: FormGroup<{
    userName: FormControl<string>;
    email: FormControl<string>;
    firstName: FormControl<string>;
    lastName: FormControl<string>;
    phoneNumber: FormControl<string>;
    password: FormControl<string>;
    roles: FormControl<string[]>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private alertService: AlertService
  ) {
    this.form = this.fb.group({
      userName: this.fb.control('', Validators.required),
      email: this.fb.control('', [Validators.required, Validators.email]),
      firstName: this.fb.control(''),
      lastName: this.fb.control(''),
      phoneNumber: this.fb.control(''),
      password: this.fb.control(''),
      roles: this.fb.control<string[]>([])
    });
  }

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.userId;

    this.userService.getAllRoles().subscribe({
      next: (roles) => { this.availableRoles = roles; }
    });

    if (this.isEdit) {
      this.form.controls.userName.disable();
      this.form.controls.password.disable();
      this.userService.getAll().subscribe({
        next: (users) => {
          const user = users.find(u => u.id === this.userId);
          if (user) this.patchForm(user);
        }
      });
    } else {
      this.form.controls.password.addValidators(Validators.required);
      this.form.controls.password.updateValueAndValidity();
    }
  }

  private patchForm(user: UserDto): void {
    this.form.patchValue({
      userName: user.userName,
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName,
      phoneNumber: user.phoneNumber,
      roles: user.roles ?? []
    });
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isSaving = true;
    const raw = this.form.getRawValue();

    if (this.isEdit) {
      this.userService.update({
        id: this.userId!,
        email: raw.email,
        firstName: raw.firstName,
        lastName: raw.lastName,
        phoneNumber: raw.phoneNumber,
        roles: raw.roles
      }).subscribe({
        next: (res) => {
          this.isSaving = false;
          if (res?.success === false) { this.alertService.showError(res.errorMessage ?? 'حدث خطأ'); return; }
          this.alertService.showSuccess('تم تحديث المستخدم بنجاح');
          this.router.navigate(['/users']);
        },
        error: () => { this.isSaving = false; this.alertService.showError('حدث خطأ أثناء التحديث'); }
      });
    } else {
      this.userService.create({
        userName: raw.userName,
        email: raw.email,
        firstName: raw.firstName,
        lastName: raw.lastName,
        phoneNumber: raw.phoneNumber,
        password: raw.password,
        roles: raw.roles
      }).subscribe({
        next: (res) => {
          this.isSaving = false;
          if (res?.success === false) { this.alertService.showError(res.message ?? 'حدث خطأ'); return; }
          // assign roles after creation
          if (raw.roles.length > 0 && res?.userId) {
            this.userService.update({ id: res.userId, email: raw.email, firstName: raw.firstName, lastName: raw.lastName, phoneNumber: raw.phoneNumber, roles: raw.roles }).subscribe();
          }
          this.alertService.showSuccess('تم إنشاء المستخدم بنجاح');
          this.router.navigate(['/users']);
        },
        error: () => { this.isSaving = false; this.alertService.showError('حدث خطأ أثناء الإنشاء'); }
      });
    }
  }

  onBack(): void {
    this.router.navigate(['/users']);
  }
}
