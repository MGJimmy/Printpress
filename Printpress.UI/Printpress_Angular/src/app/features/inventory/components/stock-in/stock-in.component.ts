import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, NonNullableFormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { AlertService } from '../../../../core/services/alert.service';
import { InventoryService } from '../../services/inventory.service';
import { PurchaseInvoiceService } from '../../services/purchase-invoice.service';
import { InventoryItemDto } from '../../models/inventory-item.dto';
import { PurchaseInvoiceCreateDto } from '../../models/purchase-invoice-create.dto';
import { AddInvoiceLineDialogComponent, AddInvoiceLineDialogResult } from '../add-invoice-line-dialog/add-invoice-line-dialog.component';

interface InvoiceLineViewModel {
  id: string;
  inventoryItemId: string;
  itemName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

@Component({
  selector: 'app-stock-in',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCardModule,
    MatIconModule,
    TableTemplateComponent
  ],
  templateUrl: './stock-in.component.html',
  styleUrl: './stock-in.component.css'
})
export class StockInComponent implements OnInit {

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  form: FormGroup<{
    invoiceNumber: FormControl<string>;
    invoiceDate: FormControl<Date | null>;
    supplierName: FormControl<string>;
  }>;

  lines: InvoiceLineViewModel[] = [];
  selectedFileName: string = '';

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'اسم الصنف', column: 'itemName' },
    { headerName: 'الكمية', column: 'quantity' },
    { headerName: 'سعر الوحدة', column: 'unitPrice' },
    { headerName: 'الإجمالي', column: 'lineTotal' }
  ];

  private inventoryItems: InventoryItemDto[] = [];
  private isSaving = false;

  constructor(
    private fb: NonNullableFormBuilder,
    private router: Router,
    private dialog: MatDialog,
    private alertService: AlertService,
    private inventoryService: InventoryService,
    private purchaseInvoiceService: PurchaseInvoiceService
  ) {
    this.form = this.fb.group({
      invoiceNumber: this.fb.control('', Validators.required),
      invoiceDate: new FormControl<Date | null>(null, Validators.required),
      supplierName: this.fb.control('', Validators.required)
    });
  }

  ngOnInit(): void {
    this.loadInventoryItems();
  }

  private loadInventoryItems(): void {
    this.inventoryService.getAll(1000, 1).subscribe({
      next: (response) => {
        console.log(response);
        this.inventoryItems = response.data.items as InventoryItemDto[];
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل عناصر المخزون');
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFileName = input.files[0].name;
    } else {
      this.selectedFileName = '';
    }
  }

  onAddLine(): void {
    const dialogRef = this.dialog.open(AddInvoiceLineDialogComponent, {
      width: '500px',
      disableClose: true,
      data: { inventoryItems: this.inventoryItems }
    });

    dialogRef.afterClosed().subscribe((result: AddInvoiceLineDialogResult | undefined) => {
      if (result) {
        this.lines = [...this.lines, {
          id: crypto.randomUUID(),
          inventoryItemId: result.inventoryItemId,
          itemName: result.inventoryItemName,
          quantity: result.quantity,
          unitPrice: result.unitPrice,
          lineTotal: result.lineTotal
        }];
      }
    });
  }

  onDeleteLine(lineId: string): void {
    this.lines = this.lines.filter(x => x.id !== lineId);
  }

  async onSave(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.lines.length === 0) {
      this.alertService.showError('يجب إضافة سطر واحد على الأقل');
      return;
    }

    if (this.isSaving) return;
    this.isSaving = true;

    try {
      let attachmentFilePath = '';

      const fileInput = this.fileInput.nativeElement;
      if (fileInput.files && fileInput.files.length > 0) {
        const response = await firstValueFrom(
          this.purchaseInvoiceService.uploadFile(fileInput.files[0])
        );
        attachmentFilePath = response.data.filePath;
        console.log(response);
        console.log(attachmentFilePath);
      }

      const formValue = this.form.getRawValue();
      const dto: PurchaseInvoiceCreateDto = {
        invoiceNumber: formValue.invoiceNumber,
        invoiceDate: formValue.invoiceDate?.toString() || '',
        supplierName: formValue.supplierName,
        attachmentFilePath,
        lines: this.lines.map(l => ({
          inventoryItemId: l.inventoryItemId,
          quantity: l.quantity,
          unitPrice: l.unitPrice
        }))
      };

      await firstValueFrom(this.purchaseInvoiceService.createInvoice(dto));

      this.alertService.showSuccess('تم حفظ الفاتورة بنجاح');
      this.router.navigate(['/inventory/items']);
    } catch {
      this.alertService.showError('حدث خطأ أثناء حفظ الفاتورة');
    } finally {
      this.isSaving = false;
    }
  }

  onCancel(): void {
    this.router.navigate(['/inventory/items']);
  }
}
