import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ErrorHandlingService {
  constructor(private toastr: ToastrService) {}

  handleError(error: any): Observable<never> {
    if (error instanceof HttpErrorResponse) {
      const errorMessage = this.getServerErrorMessage(error);
      console.error('HTTP Error:', errorMessage);
      this.toastr.error(errorMessage, 'Error');
    } else {
      console.error('Unexpected Error:', error);
      this.toastr.error('An unexpected error occurred.', 'Error');
    }
    return throwError(() => new Error('Error occurred. Please try again later.'));
  }

  private getServerErrorMessage(error: HttpErrorResponse): string {
    switch (error.status) {
      case 404:
        return `Not Found: ${error.message}`;
      case 403:
        return `Access Denied: ${error.message}`;
      case 500:
        return `Internal Server Error: ${error.message}`;
      default:
        return `Unexpected Server Error: ${error.message}`;
    }
  }
}
