import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
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

const CATEGORY_IDS: Record<string, number> = {
  Paper: 1,
  Ink: 2,
  InkSupplements: 3,
  CleaningTools: 4,
  SparePart: 6,
  Other: 999,
};

@Component({
  selector: 'app-inventory-category-item-select',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './inventory-category-item-select.component.html',
  styleUrl: './inventory-category-item-select.component.scss',
  host: {
    '[class.full-width]': 'fullWidth'
  }
})
export class InventoryCategoryItemSelectComponent implements OnInit, OnChanges, OnDestroy {
  @Input({ required: true }) itemControl!: FormControl<string>;
  @Input() fullWidth = false;
  @Input() activeOnly = true;
  @Input() initialItemId: string | null = null;
  @Output() selectedItemChange = new EventEmitter<InventoryItemDto | null>();

  categories: InventoryCategoryOption[] = [];
  items: InventoryItemDto[] = [];
  categoryId = new FormControl<number | null>(null, Validators.required);

  private destroy$ = new Subject<void>();
  private pendingItemId: string | null = null;

  constructor(
    private inventoryService: InventoryService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.itemControl.disable({ emitEvent: false });

    this.inventoryService.getCategoriesAll().subscribe({
      next: (res) => {
        this.categories = res.data ?? [];
        this.applyInitialItem();
      },
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
          const all = res.data ?? [];
          this.items = this.activeOnly ? all.filter(item => item.isActive) : all;
          this.itemControl.enable({ emitEvent: false });
          if (this.pendingItemId && this.items.some(i => i.id === this.pendingItemId)) {
            this.itemControl.setValue(this.pendingItemId);
            this.pendingItemId = null;
          }
        },
        error: () => this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون')
      });
    });

    this.itemControl.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(id => {
      this.selectedItemChange.emit(this.items.find(i => i.id === id) ?? null);
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialItemId'] && !changes['initialItemId'].firstChange) {
      this.applyInitialItem();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private applyInitialItem(): void {
    if (!this.initialItemId) return;

    this.inventoryService.getById(this.initialItemId).subscribe({
      next: (res) => {
        const item = res.data;
        if (!item) return;
        const categoryId = CATEGORY_IDS[item.inventoryItemCategory];
        if (categoryId == null) return;
        this.pendingItemId = item.id;
        this.categoryId.setValue(categoryId);
      },
      error: () => this.alertService.showError('حدث خطأ أثناء تحميل عنصر المخزون')
    });
  }
}
