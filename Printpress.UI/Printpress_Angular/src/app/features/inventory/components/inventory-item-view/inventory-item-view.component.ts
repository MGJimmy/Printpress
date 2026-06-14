import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PageChangedModel } from '../../../../shared/models/page-changed.model';
import { AlertService } from '../../../../core/services/alert.service';
import { InventoryService } from '../../services/inventory.service';
import { InventoryTransactionService } from '../../services/inventory-transaction.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { InventoryTransactionDto } from '../../models/inventory-transaction.dto';

@Component({
  selector: 'app-inventory-item-view',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    TableTemplateComponent
  ],
  templateUrl: './inventory-item-view.component.html'
})
export class InventoryItemViewComponent implements OnInit {

  item: InventoryItemDto | null = null;

  form: FormGroup<{
    name: FormControl<string>;
    inventoryItemCategory: FormControl<string>;
    packsPerCarton: FormControl<string>;
    unitsPerPack: FormControl<string>;
    expectedPurchaseLossPercent: FormControl<string>;
    expectedProductionWastePercent: FormControl<string>;
    stockQuantity: FormControl<number>;
  }>;

  transactions: InventoryTransactionDto[] = [];
  totalTransactionsCount = 0;
  currentPage = 1;
  pageSize = 10;

  filterDateFrom: Date | null = null;
  filterDateTo: Date | null = null;
  filterTransactionType: string = '';

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'نوع الحركة', 
      column: 'inventoryTransactionType',
      translationPrefix: 'inventory.enum.inventoryTransactionType' 
    },
    { headerName: 'الكمية', column: 'quantity' },
    { headerName: 'نوع المرجع', column: 'referenceType' },
    { headerName: 'العامل', column: 'workerName' },
    { headerName: 'ملاحظات', column: 'notes' },
    { headerName: 'التاريخ', column: 'createdAt' }
  ];

  private itemId!: string;

  constructor(
    private fb: NonNullableFormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private inventoryService: InventoryService,
    private inventoryTransactionService: InventoryTransactionService
  ) {
    this.form = this.fb.group({
      name: this.fb.control({ value: '', disabled: true }),
      inventoryItemCategory: this.fb.control({ value: '', disabled: true }),
      packsPerCarton: this.fb.control({ value: '', disabled: true }),
      unitsPerPack: this.fb.control({ value: '', disabled: true }),
      expectedPurchaseLossPercent: this.fb.control({ value: '', disabled: true }),
      expectedProductionWastePercent: this.fb.control({ value: '', disabled: true }),
      stockQuantity: this.fb.control({ value: 0, disabled: true })
    });
  }

  ngOnInit(): void {
    this.itemId = this.activatedRoute.snapshot.paramMap.get('id')!;
    this.loadItem();
    this.loadTransactions();
  }

  private loadItem(): void {
    this.inventoryService.getById(this.itemId).subscribe({
      next: (response) => {
        this.item = response.data;
        this.form.patchValue({
          name: this.item.name,
          inventoryItemCategory: this.item.inventoryItemCategory,
          packsPerCarton: this.item.packsPerCarton?.toString() ?? '-',
          unitsPerPack: this.item.unitsPerPack?.toString() ?? '-',
          expectedPurchaseLossPercent: this.item.expectedPurchaseLossPercent + '%',
          expectedProductionWastePercent: this.item.expectedProductionWastePercent + '%',
          stockQuantity: this.item.stockQuantity
        });
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل بيانات الصنف');
      }
    });
  }

  private loadTransactions(): void {
    const dateFrom = this.filterDateFrom ? this.filterDateFrom.toISOString().split('T')[0] : undefined;
    const dateTo = this.filterDateTo ? this.filterDateTo.toISOString().split('T')[0] : undefined;
    const type = this.filterTransactionType || undefined;
    this.inventoryTransactionService.getByItemId(this.itemId, this.currentPage, this.pageSize, dateFrom, dateTo, type).subscribe({
      next: (response) => {
        this.transactions = response.data.items as InventoryTransactionDto[];
        this.totalTransactionsCount = response.data.totalCount;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل حركات المخزون');
      }
    });
  }

  applyFilter(): void {
    this.currentPage = 1;
    this.loadTransactions();
  }

  resetFilter(): void {
    this.filterDateFrom = null;
    this.filterDateTo = null;
    this.filterTransactionType = '';
    this.currentPage = 1;
    this.loadTransactions();
  }

  onPageChanged(event: PageChangedModel): void {
    this.currentPage = event.currentPage;
    this.pageSize = event.pageSize;
    this.loadTransactions();
  }

  onBack(): void {
    this.router.navigate(['/inventory/items']);
  }

  onWorkerLinkClicked(element: any): void {
    if (element.workerId) {
      this.router.navigate(['/hr/workers', element.workerId]);
    }
  }
}
