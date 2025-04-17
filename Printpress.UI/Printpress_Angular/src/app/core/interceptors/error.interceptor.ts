import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, EMPTY } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const toastr = inject(ToastrService);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 400) {
                // Handle Bad Request errors
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
            
            // Return empty observable to complete the request without error
            return EMPTY;
        })
    );
}; 