import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { SparePartService } from '../../services/spare-part.service';
import { SparePartItemDto } from '../../models/spare-part-item.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-spare-part-item-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, TableTemplateComponent],
  templateUrl: './spare-part-item-list.component.html',
  styleUrl: './spare-part-item-list.component.scss',
})
export class SparePartItemListComponent implements OnInit {
  items: SparePartItemDto[] = [];
  totalCount = 0;

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'عبوات/كرتونة', column: 'packsPerCarton' },
    { headerName: 'وحدات/عبوة', column: 'unitsPerPack' },
    { headerName: 'دخول', column: 'totalInQuantity' },
    { headerName: 'صرف', column: 'totalOutQuantity' },
    { headerName: 'الكمية في المخزون', column: 'stockQuantity' }
  ];

  constructor(
    private sparePartService: SparePartService,
    private alertService: AlertService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadItems(DEFAULT_PAGE_SIZE, DEFAULT_PAGE_NUMBER);
  }

  private loadItems(pageSize: number, pageNumber: number) {
    this.sparePartService.getAll(pageSize, pageNumber).subscribe({
      next: (response) => {
        this.items = response.data.items as SparePartItemDto[];
        this.totalCount = response.data.totalCount;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل قطع الغيار');
      }
    });
  }

  onPageChange(event: PageChangedModel) {
    this.loadItems(event.pageSize, event.currentPage);
  }

  onAdd() {
    this.router.navigate(['/spare-parts/items/add']);
  }

  onView(id: string) {
    this.router.navigate(['/spare-parts/items/view', id]);
  }

  onEdit(id: string) {
    this.router.navigate(['/spare-parts/items/edit', id]);
  }

  onDelete(id: string) {
    if (confirm('هل أنت متأكد من حذف هذا العنصر؟')) {
      this.sparePartService.delete(id).subscribe({
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

  onStockInClick() {
    this.router.navigate(['/spare-parts/stock-in']);
  }

  onStockInInvoicesClick() {
    this.router.navigate(['/spare-parts/stock-in/invoices']);
  }

  onStockOutClick() {
    this.router.navigate(['/spare-parts/stock-out']);
  }

  onStockOutInvoicesClick() {
    this.router.navigate(['/spare-parts/stock-out/invoices']);
  }
}
