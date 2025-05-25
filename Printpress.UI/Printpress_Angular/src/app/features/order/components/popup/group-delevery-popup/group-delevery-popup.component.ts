import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { DateAdapter, MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

interface DialogData {
  deliveryDate?: Date;
  deliveredFrom?: string;
  deliveredTo?: string;
  deliveryNotes?: string;
  isViewMode?: boolean;
}

@Component({
  selector: 'app-group-delevery-popup',
  standalone: true,
  imports: [
    CommonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    MatButtonModule
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './group-delevery-popup.component.html',
  styleUrl: './group-delevery-popup.component.css'
})
export class GroupDeleveryPopupComponent {
  deliveryForm: FormGroup;
  isViewMode: boolean = false;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<GroupDeleveryPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    this.isViewMode = data.isViewMode || false;
    
    this.deliveryForm = this.fb.group({
      deliveryDate: [{ value: data.deliveryDate || '', disabled: this.isViewMode }, Validators.required],
      deliveredFrom: [{ value: data.deliveredFrom || '', disabled: this.isViewMode }, Validators.required],
      deliveredTo: [{ value: data.deliveredTo || '', disabled: this.isViewMode }, Validators.required],
      deliveryNotes: [{ value: data.deliveryNotes || '', disabled: this.isViewMode }]
    });
  }

  onSubmit(): void {
    if (this.deliveryForm.valid) {
      this.dialogRef.close(this.deliveryForm.value);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}

