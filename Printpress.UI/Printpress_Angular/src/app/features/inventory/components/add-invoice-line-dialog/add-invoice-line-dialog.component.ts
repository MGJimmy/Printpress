import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { InventoryCategoryItemSelectComponent } from '../inventory-category-item-select/inventory-category-item-select.component';

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
    MatDialogModule,
    InventoryCategoryItemSelectComponent
  ],
  templateUrl: './add-invoice-line-dialog.component.html',
  styles: [`
    .dialog-form { display: flex; flex-direction: column; }
    .full-width { width: 100%; }
  `]
})
export class AddInvoiceLineDialogComponent {
  selectedItem: InventoryItemDto | null = null;

  form: FormGroup<{
    inventoryItemId: FormControl<string>;
    quantity: FormControl<number>;
    unitPrice: FormControl<number>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    public dialogRef: MatDialogRef<AddInvoiceLineDialogComponent>
  ) {
    this.form = this.fb.group({
      inventoryItemId: this.fb.control('', Validators.required),
      quantity: this.fb.control(0, [Validators.required, Validators.min(0.01)]),
      unitPrice: this.fb.control(0, [Validators.required, Validators.min(0.01)])
    });
  }

  onSelectedItem(item: InventoryItemDto | null): void {
    this.selectedItem = item;
  }

  onConfirm(): void {
    if (this.form.invalid || !this.selectedItem) {
      this.form.markAllAsTouched();
      return;
    }

    const { inventoryItemId, quantity, unitPrice } = this.form.getRawValue();
    const result: AddInvoiceLineDialogResult = {
      inventoryItemId,
      inventoryItemName: this.selectedItem.name,
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
