import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { finalize } from 'rxjs';
import { SearchSelectComponent, SearchSelectItem } from '../../../../shared/components/search-select/search-select.component';
import { ItemServiceExecutionService } from '../../services/item-service-execution.service';
import { WorkerService } from '../../../hr/services/worker.service';
import { AlertService } from '../../../../core/services/alert.service';
import { TranslationService } from '../../../../core/services/translation.service';

export interface BatchServiceOption {
  serviceCategoryId: string;
  serviceCategoryName: string;
  remaining: number;
}

export interface ExecuteBatchDialogData {
  mode: 'item' | 'group';
  groupId?: string;
  orderItemId?: string;
  services: BatchServiceOption[];
}

@Component({
  selector: 'app-execute-batch-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    SearchSelectComponent
  ],
  templateUrl: './execute-batch-dialog.component.html',
  styleUrl: './execute-batch-dialog.component.scss'
})
export class ExecuteBatchDialogComponent implements OnInit {
  workerItems: SearchSelectItem[] = [];
  isSaving = false;

  form = this.fb.group({
    workerId: this.fb.control('', Validators.required),
    serviceCategoryIds: this.fb.control<string[]>([], [Validators.required, Validators.minLength(1)]),
    executionDate: this.fb.control<Date>(new Date(), Validators.required),
    notes: this.fb.control('')
  });

  constructor(
    private fb: NonNullableFormBuilder,
    private dialogRef: MatDialogRef<ExecuteBatchDialogComponent, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: ExecuteBatchDialogData,
    private executionService: ItemServiceExecutionService,
    private workerService: WorkerService,
    private alertService: AlertService,
    protected _t: TranslationService
  ) {}

  ngOnInit(): void {
    this.workerService.getActive().subscribe({
      next: (res) => {
        this.workerItems = res.data
          .filter(w => w.isActive)
          .map(w => ({ id: w.id, name: w.name }));
      },
      error: () => this.alertService.showError(this._t.t('orders.error_loading_workers'))
    });
  }

  get isItemMode(): boolean {
    return this.data.mode === 'item';
  }

  get canSave(): boolean {
    return this.form.valid
      && this.form.controls.serviceCategoryIds.value.length > 0
      && !this.isSaving;
  }

  onSave(): void {
    if (!this.canSave) {
      this.form.markAllAsTouched();
      return;
    }

    const val = this.form.getRawValue();
    const shared = {
      workerId: val.workerId,
      serviceCategoryIds: val.serviceCategoryIds,
      executionDate: (val.executionDate as Date).toISOString().split('T')[0],
      notes: val.notes
    };

    const request$ = this.isItemMode
      ? this.executionService.executeItemBatch({
          ...shared,
          orderItemId: this.data.orderItemId!
        })
      : this.executionService.executeGroupBatch({
          ...shared,
          groupId: this.data.groupId!
        });

    this.isSaving = true;
    request$.pipe(
      finalize(() => { this.isSaving = false; })
    ).subscribe({
      next: () => {
        this.alertService.showSuccess(this._t.t('orders.execution_saved'));
        this.dialogRef.close(true);
      },
      error: (err) => {
        const msg = err?.error?.message || this._t.t('orders.error_executing');
        this.alertService.showError(msg);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
