import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  constructor(private toastr: ToastrService) {}

  showSuccess(message: string, title?: string): void {
    this.toastr.success(message, title || 'عملية ناجحة');
  }

  showError(message: string, title?: string): void {
    this.toastr.error(message, title || 'حدث خطأ');
  }

  showInfo(message: string, title?: string): void {
    this.toastr.info(message, title || 'تم');
  }

  showWarning(message: string, title?: string): void {
    this.toastr.warning(message, title || 'تحذير');
  }
}
