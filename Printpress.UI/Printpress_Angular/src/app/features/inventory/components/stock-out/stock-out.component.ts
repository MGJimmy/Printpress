import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { AlertService } from '../../../../core/services/alert.service';
import { InventoryService } from '../../services/inventory.service';
import { InventoryTransactionService } from '../../services/inventory-transaction.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';

function maxStockValidator(getMax: () => number) {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (value === null || value === undefined || value === '') return null;
    return value > getMax() ? { maxStock: true } : null;
  };
}

@Component({
  selector: 'app-stock-out',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
  ],
  templateUrl: './stock-out.component.html',
})
export class StockOutComponent implements OnInit {
  inventoryItems: InventoryItemDto[] = [];
  selectedItem: InventoryItemDto | null = null;

  form: FormGroup<{
    inventoryItemId: FormControl<string>;
    quantity: FormControl<number | null>;
    notes: FormControl<string>;
  }>;

  get remaining(): number {
    const stock = this.selectedItem?.stockQuantity ?? 0;
    const qty = this.form.controls.quantity.value ?? 0;
    return stock - qty;
  }

  constructor(
    private fb: NonNullableFormBuilder,
    private router: Router,
    private alertService: AlertService,
    private inventoryService: InventoryService,
    private inventoryTransactionService: InventoryTransactionService
  ) {
    this.form = this.fb.group({
      inventoryItemId: this.fb.control('', Validators.required),
      quantity: new FormControl<number | null>(null, {
        validators: [
          Validators.required,
          Validators.min(1),
          maxStockValidator(() => this.selectedItem?.stockQuantity ?? 0)
        ]
      }),
      notes: this.fb.control('')
    });
  }

  ngOnInit(): void {
    this.inventoryService.getAll(1000, 1).subscribe({
      next: (response) => {
        this.inventoryItems = response.data.items as InventoryItemDto[];
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون');
      }
    });

    this.form.controls.inventoryItemId.valueChanges.subscribe(id => {
      this.selectedItem = this.inventoryItems.find(i => i.id === id) ?? null;
      this.form.controls.quantity.updateValueAndValidity();
    });
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.inventoryTransactionService.stockOut({
      inventoryItemId: raw.inventoryItemId,
      quantity: raw.quantity!,
      notes: raw.notes
    }).subscribe({
      next: () => {
        this.alertService.showSuccess('تم صرف الكمية من المخزن بنجاح');
        this.router.navigate(['/inventory/items']);
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء صرف الكمية');
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/inventory/items']);
  }
}
