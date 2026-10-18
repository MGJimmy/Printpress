import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { InventoryService } from '../../services/inventory.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';
import { MatIconModule } from '@angular/material/icon';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../../../reports/models/order-inventory-items-report.dto';

@Component({
  selector: 'app-inventory-item-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    TableTemplateComponent,
    MatIconModule,
  ],
  templateUrl: './inventory-item-list.component.html',
  styleUrl: './inventory-item-list.component.scss',
})
export class InventoryItemListComponent implements OnInit {
  items: InventoryItemDto[] = [];
  totalCount = 0;
  categories: InventoryCategoryFilterDto[] = [];
  nameItems: InventoryItemFilterDto[] = [];

  filterForm: FormGroup<{
    categoryId: FormControl<number | null>;
    itemId: FormControl<string>;
  }>;

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'الفئة', 
      column: 'inventoryItemCategory',
      translationPrefix: 'inventory.enum.inventoryItemCategory'
    },
    { headerName: 'عبوات/كرتونة', column: 'packsPerCarton' },
    { headerName: 'وحدات/عبوة', column: 'unitsPerPack' },
    { headerName: 'نسبة الفاقد الشراء %', column: 'expectedPurchaseLossPercent' },
    { headerName: 'نسبة الهالك الإنتاج %', column: 'expectedProductionWastePercent' },
    { headerName: 'دخول', column: 'totalInQuantity' },
    { headerName: 'صرف', column: 'totalOutQuantity' },
    { headerName: 'الكمية في المخزون', column: 'stockQuantity' },
    { headerName: 'الحالة', column: 'isActive', translationPrefix: 'inventory.inventoryItemActiveStatus' }
  ]; 

  private pageNumber = DEFAULT_PAGE_NUMBER;
  private pageSize = DEFAULT_PAGE_SIZE;

  constructor(
    private fb: NonNullableFormBuilder,
    private inventoryService: InventoryService,
    private alertService: AlertService,
    private router: Router 
  ) {
    this.filterForm = this.fb.group({
      categoryId: this.fb.control<number | null>(null),
      itemId: this.fb.control(''),
    });
  }

  ngOnInit() {
    this.inventoryService.getCategoriesAll().subscribe({
      next: (res) => { this.categories = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل التصنيفات'); },
    });
    this.filterForm.controls.categoryId.valueChanges.subscribe((categoryId) => {
      this.nameItems = [];
      this.filterForm.controls.itemId.setValue('', { emitEvent: false });
      if (categoryId != null) {
        this.inventoryService.getByCategory(categoryId).subscribe({
          next: (res) => { this.nameItems = (res.data ?? []).map((i) => ({ id: i.id, name: i.name })); },
          error: () => { this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون'); },
        });
      }
      this.pageNumber = DEFAULT_PAGE_NUMBER;
      this.loadItems();
    });
    this.filterForm.controls.itemId.valueChanges.subscribe(() => {
      this.pageNumber = DEFAULT_PAGE_NUMBER;
      this.loadItems();
    });
    this.loadItems();
  }

  private loadItems() {
    const v = this.filterForm.getRawValue();
    this.inventoryService.getAll(
      this.pageSize,
      this.pageNumber,
      v.categoryId,
      v.itemId || undefined,
    ).subscribe({
      next: (response) => {
        this.items = response.data.items as InventoryItemDto[];
        this.totalCount = response.data.totalCount;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون');
      }
    });
  }

  onPageChange(event: PageChangedModel) {
    this.pageSize = event.pageSize;
    this.pageNumber = event.currentPage;
    this.loadItems();
  }

  onAdd() {
    this.router.navigate(['/inventory/items/add']);
  }

  onEdit(id: string) {
    this.router.navigate(['/inventory/items/edit', id]);
  }

  onView(id: string) {
    this.router.navigate(['/inventory/items/view', id]);
  }

  onDelete(id: string) {
    if (confirm('هل أنت متأكد من حذف هذا العنصر؟')) {
      this.inventoryService.delete(id).subscribe({
        next: () => {
          this.alertService.showSuccess('تم حذف العنصر بنجاح');
          this.pageNumber = DEFAULT_PAGE_NUMBER;
          this.loadItems();
        },
        error: (err: any) => {
          const msg = err?.error?.errors?.[0] || 'حدث خطأ أثناء حذف العنصر';
          this.alertService.showError(msg);
        }
      });
    }
  }

  onDeactivate(id: string) {
    if (confirm('هل أنت متأكد من تعطيل هذا العنصر؟ سيتم تعطيل جميع الخدمات المرتبطة به.')) {
      this.inventoryService.deactivate(id).subscribe({
        next: () => {
          this.alertService.showSuccess('تم تعطيل العنصر بنجاح');
          this.pageNumber = DEFAULT_PAGE_NUMBER;
          this.loadItems();
        },
        error: (err: any) => {
          const msg = err?.error?.errors?.[0] || 'حدث خطأ أثناء تعطيل العنصر';
          this.alertService.showError(msg);
        }
      });
    }
  }

  onActivate(id: string) {
    if (confirm('هل أنت متأكد من تنشيط هذا العنصر؟')) {
      this.inventoryService.activate(id).subscribe({
        next: () => {
          this.alertService.showSuccess('تم تنشيط العنصر بنجاح');
          this.pageNumber = DEFAULT_PAGE_NUMBER;
          this.loadItems();
        },
        error: (err: any) => {
          const msg = err?.error?.errors?.[0] || 'حدث خطأ أثناء تنشيط العنصر';
          this.alertService.showError(msg);
        }
      });
    }
  }

  onStockInClick() {
    this.router.navigate(['/inventory/stock-in']);
  }

  onStockInInvoicesClick() {
    this.router.navigate(['/inventory/stock-in/invoices']);
  }

  onStockOutClick() {
    this.router.navigate(['/inventory/stock-out']);
  }

  onTransactionsClick() {
    this.router.navigate(['/inventory/transactions']);
  }
}
