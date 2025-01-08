import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogModel } from '../../core/models/confirm-dialog.model';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DialogService {
  constructor(private dialog: MatDialog) {}

  confirmDialog(data : ConfirmDialogModel) : Observable<boolean>{
    return this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data : data,
      disableClose: true, // Prevent closing by clicking outside
    }).afterClosed(); // Emits the result (true/false)
  }
}
