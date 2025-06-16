import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { CanComponentDeactivate } from '../interfaces/can-component-deactivate.interface'; 
import { ConfirmDialogComponent } from '../component/confirm-dialog/confirm-dialog.component';
import { Title } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class CanDeactivateGuard implements CanDeactivate<CanComponentDeactivate> {
  constructor(private dialog: MatDialog) {}

  canDeactivate(component: CanComponentDeactivate): Observable<boolean> | boolean {
    if (component.canDeactivate()) {
      return true;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        message: 'هنالك تغييرات لم يتم حفظها، هل انت متأكد من مغادرة الصفحة؟',
        title: 'تأكيد مغادرة الصفحة'
      },
    });

    return dialogRef.afterClosed(); // returns true or false
  }
  
}
