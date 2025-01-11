import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { AlertService } from '../../../core/services/alert.service';
import { ErrorHandlingService } from '../../../core/helpers/error-handling.service';
import { ReactiveFormsModule } from '@angular/forms';



@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage = '';
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private alertService: AlertService,
    private errorHandler: ErrorHandlingService
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onLogin(): void {
    if (this.loginForm.invalid) {
      return;
    }

      const username = this.loginForm.value.username;
      const password = this.loginForm.value.password;
    try {
      this.authService.login(username, password);
      this.alertService.showSuccess('تم تسجيل الدخول بنجاح');
      const returnUrl = this.router.routerState.snapshot.root.queryParams['returnUrl'] || '/clients';
      this.router.navigate([returnUrl]);
    } catch (error) {
      this.errorHandler.handleError(error);
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
    setTimeout(()=>{
      this.showPassword = !this.showPassword;
    }, 1000)
  }
  }
