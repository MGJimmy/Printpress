import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { InventoryConversionService } from '../../services/inventory-conversion.service';
import { InventoryCategoryItemSelectComponent } from '../inventory-category-item-select/inventory-category-item-select.component';

@Component({
  selector: 'app-usage-conversion-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    InventoryCategoryItemSelectComponent,
  ],
  templateUrl: './usage-conversion-create.component.html',
  styleUrl: '../stock-out/stock-out.component.scss',
})
export class UsageConversionCreateComponent {
  selectedItem: InventoryItemDto | null = null;
  isSaving = false;
  initialItemId: string | null = null;

  form: FormGroup<{
    inventoryItemId: FormControl<string>;
    occurredAt: FormControl<Date>;
    quantity: FormControl<number | null>;
    notes: FormControl<string>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private alertService: AlertService,
    private conversionService: InventoryConversionService,
  ) {
    this.form = this.fb.group({
      inventoryItemId: this.fb.control('', Validators.required),
      occurredAt: this.fb.control(new Date(), Validators.required),
      quantity: new FormControl<number | null>(null, {
        validators: [Validators.required, Validators.min(1)],
      }),
      notes: this.fb.control('', [Validators.required, Validators.maxLength(500)]),
    });

    const q = this.route.snapshot.queryParamMap;
    this.initialItemId = q.get('itemId');
    const quantity = q.get('quantity');
    if (quantity) {
      const n = Number(quantity);
      if (!Number.isNaN(n) && n > 0) {
        this.form.controls.quantity.setValue(n);
      }
    }
    const occurredAt = q.get('occurredAt');
    if (occurredAt) {
      const d = new Date(occurredAt);
      if (!Number.isNaN(d.getTime())) {
        this.form.controls.occurredAt.setValue(d);
      }
    }
  }

  onSelectedItem(item: InventoryItemDto | null): void {
    this.selectedItem = item;
  }

  onSave(): void {
    if (this.form.invalid || !this.selectedItem || this.isSaving) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.isSaving = true;
    this.conversionService.create({
      inventoryItemId: raw.inventoryItemId,
      quantity: raw.quantity!,
      notes: raw.notes,
      occurredAt: (raw.occurredAt as Date).toISOString(),
    }).pipe(
      finalize(() => { this.isSaving = false; }),
    ).subscribe({
      next: () => {
        this.alertService.showSuccess('تم حفظ التحويل إلى منتج');
        this.router.navigate(['/inventory/usage-conversions']);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/inventory/usage-conversions']);
  }
}
