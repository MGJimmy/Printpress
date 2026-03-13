import { Injectable } from '@angular/core';
import { CanDeactivate, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { CanComponentDeactivate } from '../interfaces/can-component-deactivate.interface';
import { ConfirmDialogComponent } from '../component/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class CanDeactivateGuard implements CanDeactivate<CanComponentDeactivate> {
  constructor(private dialog: MatDialog) {}

  canDeactivate(
    component: CanComponentDeactivate,
    _currentRoute: ActivatedRouteSnapshot,
    _currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ): Observable<boolean> | boolean {
    // Allow free navigation between order sub-routes without prompting
    if (nextState?.url.startsWith('/order/') || nextState?.url === '/order') {
      return true;
    }

    if (component.canDeactivate()) {
      return true;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        message: 'هنالك تغييرات لم يتم حفظها، هل انت متأكد من مغادرة الصفحة؟',
        title: 'تأكيد مغادرة الصفحة'
      },
    });

    return dialogRef.afterClosed();
  }

}
