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

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<GroupDeleveryPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.deliveryForm = this.fb.group({
      deliveryDate: ['', Validators.required],
      deliveredFrom: ['', Validators.required],
      deliveredTo: ['', Validators.required],
      deliveryNotes: ['']
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

