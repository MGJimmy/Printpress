import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, NonNullableFormBuilder, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ItemServiceExecutionService } from '../../services/item-service-execution.service';
import { WorkerService } from '../../../hr/services/worker.service';
import { WorkerDto } from '../../../hr/models/worker.dto';
import {
  ItemExecutionSummaryDto,
  ServiceProgressDto,
  ExecuteServiceRequestDto
} from '../../models/execution/execution.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { TranslationService } from '../../../../core/services/translation.service';

@Component({
  selector: 'app-execute-item-service',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './execute-item-service.component.html'
})
export class ExecuteItemServiceComponent implements OnInit {
  itemId!: string;
  groupId!: string;
  itemSummary: ItemExecutionSummaryDto | null = null;
  workers: WorkerDto[] = [];
  selectedService: ServiceProgressDto | null = null;
  isSaving = false;

  form = this.fb.group({
    serviceCategoryId: this.fb.control<string>('', Validators.required),
    executionDate: this.fb.control<Date>(new Date(), Validators.required),
    notes: this.fb.control<string>(''),
    workerRows: this.fb.array<ReturnType<typeof this.createWorkerRow>>([])
  });

  get workerRows(): FormArray {
    return this.form.controls.workerRows;
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: NonNullableFormBuilder,
    private executionService: ItemServiceExecutionService,
    private workerService: WorkerService,
    private alertService: AlertService,
    private _t: TranslationService
  ) {}

  ngOnInit(): void {
    this.itemId = this.route.snapshot.paramMap.get('itemId')!;
    this.groupId = this.route.snapshot.paramMap.get('groupId')!;
    this.loadData();
  }

  private loadData(): void {
    this.executionService.getItemSummary(this.itemId).subscribe({
      next: (res) => {
        this.itemSummary = res.data;
      },
      error: () => this.alertService.showError(this._t.t('orders.error_loading_item'))
    });

    this.workerService.getAll().subscribe({
      next: (res) => {
        this.workers = res.data.filter(w => w.isActive);
      },
      error: () => this.alertService.showError(this._t.t('orders.error_loading_workers'))
    });
  }

  onServiceSelect(serviceCategoryId: string): void {
    this.selectedService = this.itemSummary?.serviceProgresses.find(
      s => s.serviceCategoryId === serviceCategoryId
    ) ?? null;
  }

  get remaining(): number {
    if (!this.selectedService) return 0;
    return this.selectedService.total - this.selectedService.executed;
  }

  createWorkerRow() {
    return this.fb.group({
      workerId: this.fb.control<string>('', Validators.required),
      quantity: this.fb.control<number>(1, [Validators.required, Validators.min(1)])
    });
  }

  addWorkerRow(): void {
    this.workerRows.push(this.createWorkerRow());
  }

  removeWorkerRow(index: number): void {
    this.workerRows.removeAt(index);
  }

  getServiceStatusClass(svc: ServiceProgressDto): string {
    if (svc.isCompleted) return 'text-success';
    if (svc.executed > 0) return 'text-warning';
    return 'text-muted';
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (this.workerRows.length === 0) {
      this.alertService.showError(this._t.t('orders.worker_required'));
      return;
    }

    const val = this.form.getRawValue();
    const payload: ExecuteServiceRequestDto = {
      orderItemId: this.itemId,
      serviceCategoryId: val.serviceCategoryId,
      executionDate: (val.executionDate as Date).toISOString().split('T')[0],
      notes: val.notes,
      workers: val.workerRows.map(r => ({ workerId: r.workerId, quantity: r.quantity }))
    };

    this.isSaving = true;
    this.executionService.execute(payload).subscribe({
      next: () => {
        this.alertService.showSuccess(this._t.t('orders.execution_saved'));
        this.router.navigate([`/order/groups/${this.groupId}/items`]);
      },
      error: (err) => {
        this.isSaving = false;
        const msg = err?.error?.message || this._t.t('orders.error_executing');
        this.alertService.showError(msg);
      }
    });
  }

  onBack(): void {
    this.router.navigate([`/order/groups/${this.groupId}/items`]);
  }
}
