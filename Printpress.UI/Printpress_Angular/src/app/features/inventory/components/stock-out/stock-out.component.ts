import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { InventoryTransactionService } from '../../services/inventory-transaction.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { WorkerService } from '../../../hr/services/worker.service';
import { WorkerDto } from '../../../hr/models/worker.dto';
import { SearchSelectComponent, SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';
import { InventoryCategoryItemSelectComponent } from '../inventory-category-item-select/inventory-category-item-select.component';

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
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    SearchSelectComponent,
    InventoryCategoryItemSelectComponent,
  ],
  templateUrl: './stock-out.component.html',
  styleUrl: './stock-out.component.scss'
})
export class StockOutComponent implements OnInit {
  selectedItem: InventoryItemDto | null = null;
  workers: WorkerDto[] = [];
  workerItems: SearchSelectItem[] = [];
  isSaving = false;

  form: FormGroup<{
    inventoryItemId: FormControl<string>;
    occurredAt: FormControl<Date>;
    quantity: FormControl<number | null>;
    notes: FormControl<string>;
    workerId: FormControl<string | null>;
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
    private inventoryTransactionService: InventoryTransactionService,
    private workerService: WorkerService
  ) {
    this.form = this.fb.group({
      inventoryItemId: this.fb.control('', Validators.required),
      occurredAt: this.fb.control(new Date(), Validators.required),
      quantity: new FormControl<number | null>(null, {
        validators: [
          Validators.required,
          Validators.min(1),
          maxStockValidator(() => this.selectedItem?.stockQuantity ?? 0)
        ]
      }),
      notes: this.fb.control(''),
      workerId: new FormControl<string | null>(null)
    });
  }

  ngOnInit(): void {
    this.workerService.getActive().subscribe({
      next: res => {
        this.workers = res.data;
        this.workerItems = this.workers.map(w => ({ id: w.id, name: w.name }));
      }
    });

    this.form.controls.inventoryItemId.valueChanges.subscribe(() => {
      this.form.controls.quantity.updateValueAndValidity();
    });
  }

  onSelectedItem(item: InventoryItemDto | null): void {
    this.selectedItem = item;
    this.form.controls.quantity.updateValueAndValidity();
  }

  onSave(): void {
    if (this.form.invalid || !this.selectedItem || this.isSaving) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.isSaving = true;
    this.inventoryTransactionService.stockOut({
      inventoryItemId: raw.inventoryItemId,
      quantity: raw.quantity!,
      notes: raw.notes,
      workerId: raw.workerId ?? undefined,
      occurredAt: (raw.occurredAt as Date).toISOString()
    }).pipe(
      finalize(() => { this.isSaving = false; })
    ).subscribe({
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
