import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { InventoryItemSelectionDto } from '../../models/inventory-item-selection.dto';

export interface AddInvoiceLineDialogData {
  inventoryItems: InventoryItemSelectionDto[];
}

export interface AddInvoiceLineDialogResult {
  inventoryItemId: string;
  inventoryItemName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

@Component({
  selector: 'app-add-invoice-line-dialog',
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
  templateUrl: './add-invoice-line-dialog.component.html'
})
export class AddInvoiceLineDialogComponent {
  form: FormGroup<{
    inventoryItemId: FormControl<string>;
    quantity: FormControl<number>;
    unitPrice: FormControl<number>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    public dialogRef: MatDialogRef<AddInvoiceLineDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AddInvoiceLineDialogData
  ) {
    this.form = this.fb.group({
      inventoryItemId: this.fb.control('', Validators.required),
      quantity: this.fb.control(0, [Validators.required, Validators.min(0.01)]),
      unitPrice: this.fb.control(0, [Validators.required, Validators.min(0.01)])
    });
  }

  onConfirm(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { inventoryItemId, quantity, unitPrice } = this.form.getRawValue();
    const item = this.data.inventoryItems.find(x => x.id === inventoryItemId)!;

    const result: AddInvoiceLineDialogResult = {
      inventoryItemId,
      inventoryItemName: item.name,
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
