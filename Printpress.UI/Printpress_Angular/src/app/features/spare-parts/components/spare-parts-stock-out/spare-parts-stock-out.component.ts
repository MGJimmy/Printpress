import { Component, OnInit } from '@angular/core';
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
import { SparePartService } from '../../services/spare-part.service';
import { SparePartSellingInvoiceService } from '../../services/spare-part-selling-invoice.service';
import { SparePartItemDto } from '../../models/spare-part-item.dto';
import { SparePartSellingInvoiceCreateDto } from '../../models/spare-part-selling-invoice-create.dto';
import { AddSparePartLineDialogComponent, AddSparePartLineDialogResult } from '../add-spare-part-line-dialog/add-spare-part-line-dialog.component';

interface SellingLineViewModel {
  id: string;
  sparePartItemId: string;
  itemName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

@Component({
  selector: 'app-spare-parts-stock-out',
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
  templateUrl: './spare-parts-stock-out.component.html'
})
export class SparePartsStockOutComponent implements OnInit {
  form: FormGroup<{
    clientName: FormControl<string>;
    invoiceDate: FormControl<Date | null>;
  }>;

  lines: SellingLineViewModel[] = [];

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'اسم القطعة', column: 'itemName' },
    { headerName: 'الكمية', column: 'quantity' },
    { headerName: 'سعر الوحدة', column: 'unitPrice' },
    { headerName: 'الإجمالي', column: 'lineTotal' }
  ];

  private sparePartItems: SparePartItemDto[] = [];
  private isSaving = false;

  constructor(
    private fb: NonNullableFormBuilder,
    private router: Router,
    private dialog: MatDialog,
    private alertService: AlertService,
    private sparePartService: SparePartService,
    private sellingInvoiceService: SparePartSellingInvoiceService
  ) {
    this.form = this.fb.group({
      clientName: this.fb.control('', Validators.required),
      invoiceDate: new FormControl<Date | null>(null, Validators.required)
    });
  }

  ngOnInit(): void {
    this.sparePartService.getAllForSelection().subscribe({
      next: (response) => {
        this.sparePartItems = response.data ?? [];
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل قطع الغيار');
      }
    });
  }

  get totalAmount(): number {
    return this.lines.reduce((sum, l) => sum + l.lineTotal, 0);
  }

  onAddLine(): void {
    const dialogRef = this.dialog.open(AddSparePartLineDialogComponent, {
      width: '500px',
      disableClose: true,
      data: { items: this.sparePartItems, validationMode: 'stock-out' }
    });

    dialogRef.afterClosed().subscribe((result: AddSparePartLineDialogResult | undefined) => {
      if (result) {
        this.lines = [...this.lines, {
          id: crypto.randomUUID(),
          sparePartItemId: result.sparePartItemId,
          itemName: result.itemName,
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
      const formValue = this.form.getRawValue();
      const dto: SparePartSellingInvoiceCreateDto = {
        clientName: formValue.clientName,
        invoiceDate: formValue.invoiceDate?.toISOString() || '',
        lines: this.lines.map(l => ({
          sparePartItemId: l.sparePartItemId,
          quantity: l.quantity,
          unitPrice: l.unitPrice
        }))
      };

      await firstValueFrom(this.sellingInvoiceService.createInvoice(dto));
      this.alertService.showSuccess('تم حفظ فاتورة البيع بنجاح');
      this.router.navigate(['/spare-parts/items']);
    } catch {
      this.alertService.showError('حدث خطأ أثناء حفظ فاتورة البيع');
    } finally {
      this.isSaving = false;
    }
  }

  onCancel(): void {
    this.router.navigate(['/spare-parts/items']);
  }
}
