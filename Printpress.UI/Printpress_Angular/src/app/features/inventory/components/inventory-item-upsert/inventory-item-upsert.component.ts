import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { forkJoin } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { InventoryService, InventoryItemUpsertDto } from '../../services/inventory.service';

interface CategoryOption {
  value: string;
  label: string;
}

@Component({
  selector: 'app-inventory-item-upsert',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './inventory-item-upsert.component.html'
})
export class InventoryItemUpsertComponent implements OnInit {

  isEditMode = false;
  isLimited = false;
  itemId: string | null = null;

  categories: CategoryOption[] = [
    { value: 'Paper', label: 'ورق' },
    { value: 'Ink', label: 'حبر' },
    { value: 'InkSupplements', label: 'مواد مساعدة للحبر' },
    { value: 'CleaningTools', label: 'أدوات تنظيف' },
    { value: 'SparePart', label: 'قطع غيار' },
    { value: 'Other', label: 'أخرى' }
  ];

  form: FormGroup<{
    name: FormControl<string>;
    inventoryItemCategory: FormControl<string>;
    packsPerCarton: FormControl<number | null>;
    unitsPerPack: FormControl<number | null>;
    expectedPurchaseLossPercent: FormControl<number>;
    expectedProductionWastePercent: FormControl<number>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private inventoryService: InventoryService
  ) {
    this.form = this.fb.group({
      name: this.fb.control('', Validators.required),
      inventoryItemCategory: this.fb.control('', Validators.required),
      packsPerCarton: new FormControl<number | null>(null),
      unitsPerPack: new FormControl<number | null>(null),
      expectedPurchaseLossPercent: this.fb.control(0, [Validators.required, Validators.min(0), Validators.max(100)]),
      expectedProductionWastePercent: this.fb.control(0, [Validators.required, Validators.min(0), Validators.max(100)])
    });
  }

  ngOnInit(): void {
    this.itemId = this.activatedRoute.snapshot.paramMap.get('id');
    this.isEditMode = !!this.itemId;

    if (this.isEditMode) {
      this.loadEditData();
    }
  }

  private loadEditData(): void {
      this.inventoryService.getById(this.itemId!).subscribe({
      next: (res) => {
        const data = res.data;
        this.form.patchValue({
          name: data.name,
          inventoryItemCategory: data.inventoryItemCategory,
          packsPerCarton: data.packsPerCarton,
          unitsPerPack: data.unitsPerPack,
          expectedPurchaseLossPercent: data.expectedPurchaseLossPercent,
          expectedProductionWastePercent: data.expectedProductionWastePercent
        });

        this.isLimited = data.hasTransactions;
        if (this.isLimited) {
          this.form.controls.inventoryItemCategory.disable();
          this.form.controls.packsPerCarton.disable();
          this.form.controls.unitsPerPack.disable();
        }
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل بيانات الصنف');
      }
    });
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const payload: InventoryItemUpsertDto = {
      name: raw.name,
      inventoryItemCategory: raw.inventoryItemCategory,
      packsPerCarton: raw.packsPerCarton,
      unitsPerPack: raw.unitsPerPack,
      expectedPurchaseLossPercent: raw.expectedPurchaseLossPercent,
      expectedProductionWastePercent: raw.expectedProductionWastePercent
    };

    const request$ = this.isEditMode
      ? this.inventoryService.update(this.itemId!, payload)
      : this.inventoryService.add(payload);

    request$.subscribe({
      next: () => {
        const msg = this.isEditMode ? 'تم تعديل الصنف بنجاح' : 'تم إضافة الصنف بنجاح';
        this.alertService.showSuccess(msg);
        this.router.navigate(['/inventory/items']);
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء حفظ البيانات');
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/inventory/items']);
  }
}
