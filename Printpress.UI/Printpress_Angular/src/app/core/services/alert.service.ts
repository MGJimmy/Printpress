import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  constructor(private toastr: ToastrService) {}

  showSuccess(message: string, title?: string): void {
    this.toastr.success(message, title || 'عملية ناجحة', { timeOut: 3000 });
  }

  showError(message: string, title?: string): void {
    this.toastr.error(message, title || 'حدث خطأ',  
  {
    timeOut: 0,
    extendedTimeOut: 0,
    closeButton: true
  });
  }

  showInfo(message: string, title?: string): void {
    this.toastr.info(message, title || 'تم', { timeOut: 3000 });
  }

  showWarning(message: string, title?: string): void {
    this.toastr.warning(message, title || 'تحذير', { timeOut: 3000 });
  }
}
