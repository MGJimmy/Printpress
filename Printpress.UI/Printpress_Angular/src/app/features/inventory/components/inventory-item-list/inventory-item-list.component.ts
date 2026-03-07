import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { InventoryService } from '../../services/inventory.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-inventory-item-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, TableTemplateComponent],
  templateUrl: './inventory-item-list.component.html',
})
export class InventoryItemListComponent implements OnInit {
  items: InventoryItemDto[] = [];
  totalCount = 0;

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'الفئة', column: 'inventoryItemCategory' },
    { headerName: 'عبوات/كرتونة', column: 'packsPerCarton' },
    { headerName: 'وحدات/عبوة', column: 'unitsPerPack' },
    { headerName: 'نسبة الفاقد الشراء %', column: 'expectedPurchaseLossPercent' },
    { headerName: 'نسبة الهالك الإنتاج %', column: 'expectedProductionWastePercent' },
  ];

  constructor(
    private inventoryService: InventoryService,
    private alertService: AlertService,
  ) {}

  ngOnInit() {
    this.loadItems(DEFAULT_PAGE_SIZE, DEFAULT_PAGE_NUMBER);
  }

  private loadItems(pageSize: number, pageNumber: number) {
    this.inventoryService.getAll(pageSize, pageNumber).subscribe({
      next: (response) => {
        console.log(response);
        this.items = response.data.items as InventoryItemDto[];
        this.totalCount = response.data.totalCount;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون');
      }
    });
  }

  onPageChange(event: PageChangedModel) {
    this.loadItems(event.pageSize, event.currentPage);
  }

  onAdd() {
    // TODO: navigate to add form when implemented
  }

  onEdit(id: string) {
    // TODO: navigate to edit form when implemented
  }

  onDelete(id: string) {
    if (confirm('هل أنت متأكد من حذف هذا العنصر؟')) {
      this.inventoryService.delete(id).subscribe({
        next: () => {
          this.alertService.showSuccess('تم حذف العنصر بنجاح');
          this.loadItems(DEFAULT_PAGE_SIZE, DEFAULT_PAGE_NUMBER);
        },
        error: () => {
          this.alertService.showError('حدث خطأ أثناء حذف العنصر');
        }
      });
    }
  }
}
