import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { SparePartItemDto } from '../../models/spare-part-item.dto';

export interface AddSparePartLineDialogData {
  items: SparePartItemDto[];
}

export interface AddSparePartLineDialogResult {
  sparePartItemId: string;
  itemName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

@Component({
  selector: 'app-add-spare-part-line-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDialogModule
  ],
  templateUrl: './add-spare-part-line-dialog.component.html'
})
export class AddSparePartLineDialogComponent {
  form: FormGroup<{
    sparePartItemId: FormControl<string>;
    quantity: FormControl<number>;
    unitPrice: FormControl<number>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    public dialogRef: MatDialogRef<AddSparePartLineDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AddSparePartLineDialogData
  ) {
    this.form = this.fb.group({
      sparePartItemId: this.fb.control('', Validators.required),
      quantity: this.fb.control(0, [Validators.required, Validators.min(0.01)]),
      unitPrice: this.fb.control(0, [Validators.required, Validators.min(0.01)])
    });
  }

  onConfirm(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { sparePartItemId, quantity, unitPrice } = this.form.getRawValue();
    const item = this.data.items.find(x => x.id === sparePartItemId)!;

    const result: AddSparePartLineDialogResult = {
      sparePartItemId,
      itemName: item.name,
      quantity,
      unitPrice,
      lineTotal: quantity * unitPrice
    };

    this.dialogRef.close(result);
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
