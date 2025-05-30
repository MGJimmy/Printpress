import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, EMPTY } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const toastr = inject(ToastrService);
    const authService = inject(AuthService);
    const router = inject(Router);
    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            switch (error.status) {
                case 400:
                    handle400(error, toastr);
                    break;
                case 401:
                    handle401(toastr, authService, router);
                    break;
                case 403:
                    handle403(router);
                    break;
                case 500:
                    handle500(error, toastr);
                    break;
                default:
                    toastr.error('An unexpected error occurred.', 'Error');
            }
            // Return empty observable to complete the request without error
            return EMPTY;
        })
    );
}; 

function handle400(error: HttpErrorResponse, toastr: ToastrService) {
    if (error.error && typeof error.error === 'object') {
        // Handle validation errors (usually returned as object with error messages)
        const errorMessages = [];
        
        // Extract error messages from response
        if (error.error.errors) {
            // Handle ASP.NET validation errors format
            for (const key in error.error.errors) {
                if (error.error.errors.hasOwnProperty(key)) {
                    const messages = error.error.errors[key];
                    if (Array.isArray(messages)) {
                        errorMessages.push(...messages);
                    }else{
                        errorMessages.push(messages);
                    }
                }
            }
        } else if (error.error.message) {
            // Handle single error message
            errorMessages.push(error.error.message);
        } else {
            // Generic error for unknown format
            errorMessages.push('An error occurred. Please try again.');
        }
        
        // Show each error message
        errorMessages.forEach(message => {
            toastr.error(message, 'Error');
        });
    } else {
        // Generic bad request error
        toastr.error('Bad request. Please check your input.', 'Error');
    }
}

function handle401(toastr:ToastrService, authService: AuthService, router: Router) {
  authService.logout();
  router.navigate(['/login']);
  toastr.error('You are not authorized. Please log in again.', 'Unauthorized');
}

function handle403(router: Router) {
  router.navigate(['/unauthorized']);
}

function handle500(error: HttpErrorResponse, toastr:ToastrService) {
  console.error('Server error:', error.message);
  toastr.error('An unexpected error occurred.', 'Error');
}