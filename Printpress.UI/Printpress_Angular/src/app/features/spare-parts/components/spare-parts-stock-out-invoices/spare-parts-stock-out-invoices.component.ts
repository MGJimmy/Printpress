import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, NonNullableFormBuilder } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert.service';
import { SparePartSellingInvoiceService } from '../../services/spare-part-selling-invoice.service';
import { SparePartService } from '../../services/spare-part.service';
import { SparePartItemDto } from '../../models/spare-part-item.dto';
import { SparePartSellingInvoiceListDto } from '../../models/spare-part-invoice-list.dto';

@Component({
  selector: 'app-spare-parts-stock-out-invoices',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './spare-parts-stock-out-invoices.component.html',
  styleUrl: '../spare-part-invoice-list.shared.scss',
})
export class SparePartsStockOutInvoicesComponent implements OnInit {
  items: SparePartItemDto[] = [];
  report: SparePartSellingInvoiceListDto | null = null;
  isLoading = false;
  focusedInvoiceId: string | null = null;
  lineColumns = ['itemName', 'packsPerCarton', 'unitsPerPack', 'quantity', 'unitPrice', 'lineTotal'];

  filterForm: FormGroup<{
    itemId: FormControl<string>;
    dateFrom: FormControl<Date | null>;
    dateTo: FormControl<Date | null>;
  }>;

  constructor(
    private fb: NonNullableFormBuilder,
    private invoiceService: SparePartSellingInvoiceService,
    private sparePartService: SparePartService,
    private alertService: AlertService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.filterForm = this.fb.group({
      itemId: this.fb.control(''),
      dateFrom: this.fb.control<Date | null>(null),
      dateTo: this.fb.control<Date | null>(null),
    });
  }

  ngOnInit(): void {
    this.focusedInvoiceId = this.route.snapshot.queryParamMap.get('invoiceId');
    this.sparePartService.getAllForSelection().subscribe({
      next: (res) => { this.items = res.data ?? []; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل قطع الغيار'); },
    });
    this.search();
  }

  search(): void {
    const v = this.filterForm.getRawValue();
    const from = this.asDate(v.dateFrom);
    const to = this.asDate(v.dateTo);
    if (from && to && from > to) {
      this.alertService.showError('تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له');
      return;
    }

    this.isLoading = true;
    this.invoiceService.getAll(
      v.itemId || undefined,
      from ? this.toIsoDate(from) : undefined,
      to ? this.toIsoDate(to) : undefined,
    ).pipe(
      finalize(() => { this.isLoading = false; }),
    ).subscribe({
      next: (res) => {
        this.report = res.data;
        this.scrollToFocused();
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل فواتير الصرف'); },
    });
  }

  reset(): void {
    this.filterForm.reset({
      itemId: '',
      dateFrom: null,
      dateTo: null,
    });
    this.search();
  }

  goCreate(): void {
    this.router.navigate(['/spare-parts/stock-out']);
  }

  goItems(): void {
    this.router.navigate(['/spare-parts/items']);
  }

  lineQtyTotal(lines: { quantity: number }[]): number {
    return lines.reduce((sum, line) => sum + (line.quantity || 0), 0);
  }

  private scrollToFocused(): void {
    if (!this.focusedInvoiceId) return;
    setTimeout(() => {
      document.querySelector('.invoice-card.focused')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
    });
  }

  private asDate(value: Date | null): Date | null {
    if (!value) return null;
    const date = value instanceof Date ? value : new Date(value);
    return Number.isNaN(date.getTime()) ? null : date;
  }

  private toIsoDate(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }
}
