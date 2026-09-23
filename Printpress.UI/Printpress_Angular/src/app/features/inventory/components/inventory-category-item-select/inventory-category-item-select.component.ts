import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { Subject, takeUntil } from 'rxjs';
import { InventoryService } from '../../services/inventory.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { AlertService } from '../../../../core/services/alert.service';

export interface InventoryCategoryOption {
  id: number;
  name: string;
}

@Component({
  selector: 'app-inventory-category-item-select',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './inventory-category-item-select.component.html',
  styleUrl: './inventory-category-item-select.component.scss'
})
export class InventoryCategoryItemSelectComponent implements OnInit, OnDestroy {
  @Input({ required: true }) itemControl!: FormControl<string>;
  @Input() fullWidth = false;
  @Output() selectedItemChange = new EventEmitter<InventoryItemDto | null>();

  categories: InventoryCategoryOption[] = [];
  items: InventoryItemDto[] = [];
  categoryId = new FormControl<number | null>(null, Validators.required);

  private destroy$ = new Subject<void>();

  constructor(
    private inventoryService: InventoryService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.itemControl.disable({ emitEvent: false });

    this.inventoryService.getCategoriesAll().subscribe({
      next: (res) => { this.categories = res.data ?? []; },
      error: () => this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات')
    });

    this.categoryId.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(categoryId => {
      this.items = [];
      this.itemControl.setValue('');
      this.selectedItemChange.emit(null);
      if (categoryId == null) {
        this.itemControl.disable({ emitEvent: false });
        return;
      }
      this.inventoryService.getByCategory(categoryId).subscribe({
        next: (res) => {
          if (this.categoryId.value !== categoryId) return;
          this.items = (res.data ?? []).filter(item => item.isActive);
          this.itemControl.enable({ emitEvent: false });
        },
        error: () => this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون')
      });
    });

    this.itemControl.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(id => {
      this.selectedItemChange.emit(this.items.find(i => i.id === id) ?? null);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
