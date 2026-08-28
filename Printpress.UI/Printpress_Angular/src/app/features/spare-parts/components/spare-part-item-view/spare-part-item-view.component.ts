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
import { SparePartService } from '../../services/spare-part.service';
import { SparePartTransactionService } from '../../services/spare-part-transaction.service';
import { SparePartItemDto } from '../../models/spare-part-item.dto';
import { SparePartTransactionDto } from '../../models/spare-part-transaction.dto';

@Component({
  selector: 'app-spare-part-item-view',
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
  templateUrl: './spare-part-item-view.component.html'
})
export class SparePartItemViewComponent implements OnInit {

  item: SparePartItemDto | null = null;

  form: FormGroup<{
    name: FormControl<string>;
    packsPerCarton: FormControl<string>;
    unitsPerPack: FormControl<string>;
    totalInQuantity: FormControl<number>;
    totalOutQuantity: FormControl<number>;
    stockQuantity: FormControl<number>;
  }>;

  transactions: SparePartTransactionDto[] = [];
  totalTransactionsCount = 0;
  currentPage = 1;
  pageSize = 10;

  filterDateFrom: Date | null = null;
  filterDateTo: Date | null = null;
  filterTransactionType: string = '';

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'نوع الحركة', column: 'inventoryTransactionType' },
    { headerName: 'الكمية', column: 'quantity' },
    { headerName: 'ملاحظات', column: 'notes' },
    { headerName: 'التاريخ', column: 'createdAt' }
  ];

  private itemId!: string;

  constructor(
    private fb: NonNullableFormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private sparePartService: SparePartService,
    private sparePartTransactionService: SparePartTransactionService
  ) {
    this.form = this.fb.group({
      name: this.fb.control({ value: '', disabled: true }),
      packsPerCarton: this.fb.control({ value: '', disabled: true }),
      unitsPerPack: this.fb.control({ value: '', disabled: true }),
      totalInQuantity: this.fb.control({ value: 0, disabled: true }),
      totalOutQuantity: this.fb.control({ value: 0, disabled: true }),
      stockQuantity: this.fb.control({ value: 0, disabled: true })
    });
  }

  ngOnInit(): void {
    this.itemId = this.activatedRoute.snapshot.paramMap.get('id')!;
    this.loadItem();
    this.loadTransactions();
  }

  private loadItem(): void {
    this.sparePartService.getById(this.itemId).subscribe({
      next: (response) => {
        this.item = response.data;
        this.form.patchValue({
          name: this.item.name,
          packsPerCarton: this.item.packsPerCarton?.toString() ?? '-',
          unitsPerPack: this.item.unitsPerPack?.toString() ?? '-',
          totalInQuantity: this.item.totalInQuantity,
          totalOutQuantity: this.item.totalOutQuantity,
          stockQuantity: this.item.stockQuantity
        });
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل بيانات قطعة الغيار');
      }
    });
  }

  private loadTransactions(): void {
    const dateFrom = this.filterDateFrom ? this.filterDateFrom.toISOString().split('T')[0] : undefined;
    const dateTo = this.filterDateTo ? this.filterDateTo.toISOString().split('T')[0] : undefined;
    const type = this.filterTransactionType || undefined;
    this.sparePartTransactionService.getByItemId(this.itemId, this.currentPage, this.pageSize, dateFrom, dateTo, type).subscribe({
      next: (response) => {
        this.transactions = response.data.items as SparePartTransactionDto[];
        this.totalTransactionsCount = response.data.totalCount;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل حركات قطع الغيار');
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
    this.router.navigate(['/spare-parts/items']);
  }
}
