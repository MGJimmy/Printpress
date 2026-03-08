import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
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
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
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
  }>;

  transactions: InventoryTransactionDto[] = [];
  totalTransactionsCount = 0;
  currentPage = 1;
  pageSize = 10;

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'نوع الحركة', column: 'inventoryTransactionType' },
    { headerName: 'الكمية', column: 'quantity' },
    { headerName: 'نوع المرجع', column: 'referenceType' },
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
      expectedProductionWastePercent: this.fb.control({ value: '', disabled: true })
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
          expectedProductionWastePercent: this.item.expectedProductionWastePercent + '%'
        });
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل بيانات الصنف');
      }
    });
  }

  private loadTransactions(): void {
    this.inventoryTransactionService.getByItemId(this.itemId, this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.transactions = response.data.items as InventoryTransactionDto[];
        this.totalTransactionsCount = response.data.totalCount;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل حركات المخزون');
      }
    });
  }

  onPageChanged(event: PageChangedModel): void {
    this.currentPage = event.currentPage;
    this.pageSize = event.pageSize;
    this.loadTransactions();
  }

  onBack(): void {
    this.router.navigate(['/inventory/items']);
  }
}
