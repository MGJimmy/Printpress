import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogModel } from '../../core/models/confirm-dialog.model';
import { ConfirmDialogComponent } from '../../core/component/confirm-dialog/confirm-dialog.component';
import { firstValueFrom, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DialogService {
  constructor(private dialog: MatDialog) {}

  confirmDialog(data : ConfirmDialogModel) : Observable<boolean>{
    return this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data : data,
      disableClose: true, // Prevent closing by clicking outside,
      ariaLabel: data?.title || 'Confirmation Dialog',
    }).afterClosed(); // Emits the result (true/false)
  }

  public async confirmOnBackButton(): Promise<boolean>{
    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد الرجوع',
      message: 'ستفقد جميع تعديلاتك في هذه الصفحه تأكيد الرجوع؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    return await firstValueFrom(this.confirmDialog(dialogData));
  }
}
