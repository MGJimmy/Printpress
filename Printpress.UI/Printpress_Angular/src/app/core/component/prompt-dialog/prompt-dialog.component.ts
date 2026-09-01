import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { PromptDialogModel } from '../../models/prompt-dialog.model';

@Component({
  selector: 'app-prompt-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    FontAwesomeModule,
  ],
  templateUrl: './prompt-dialog.component.html',
  styleUrls: ['./prompt-dialog.component.css'],
})
export class PromptDialogComponent {
  faXmark = faXmark;
  reasonControl: FormControl<string>;
  maxLength: number;

  constructor(
    public dialogRef: MatDialogRef<PromptDialogComponent, string | null>,
    @Inject(MAT_DIALOG_DATA) public data: PromptDialogModel,
  ) {
    this.maxLength = data.maxLength ?? 500;
    this.reasonControl = new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(this.maxLength)],
    });
  }

  confirm(): void {
    if (this.reasonControl.invalid) {
      this.reasonControl.markAsTouched();
      return;
    }
    this.dialogRef.close(this.reasonControl.value.trim());
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}
