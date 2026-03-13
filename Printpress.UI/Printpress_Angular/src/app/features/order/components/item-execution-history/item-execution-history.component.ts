import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { ItemServiceExecutionService } from '../../services/item-service-execution.service';
import { ItemExecutionHistoryDto, ItemStatusLabels } from '../../models/execution/execution.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-item-execution-history',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    TableTemplateComponent
  ],
  templateUrl: './item-execution-history.component.html'
})
export class ItemExecutionHistoryComponent implements OnInit {
  itemId!: string;
  groupId!: string;
  history: ItemExecutionHistoryDto | null = null;

  itemStatusLabels = ItemStatusLabels;

  flatProgress: any[] = [];
  flatRecords: any[] = [];

  progressColDefs: TableColDefinitionModel[] = [
    { column: 'serviceCategoryName', headerName: 'الخدمة' },
    { column: 'progress', headerName: 'المنجز / الإجمالي' },
    { column: 'remaining', headerName: 'المتبقي' }
  ];

  recordColDefs: TableColDefinitionModel[] = [
    { column: 'workerName', headerName: 'العامل' },
    { column: 'serviceCategoryName', headerName: 'الخدمة' },
    { column: 'quantity', headerName: 'الكمية' },
    { column: 'executionDate', headerName: 'تاريخ التنفيذ' },
    { column: 'notes', headerName: 'ملاحظات' }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private executionService: ItemServiceExecutionService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.itemId = this.route.snapshot.paramMap.get('itemId')!;
    this.groupId = this.route.snapshot.paramMap.get('groupId')!;
    this.executionService.getItemHistory(this.itemId).subscribe({
      next: (res) => {
        this.history = res.data;
        this.flatProgress = res.data.serviceProgresses.map(s => ({
          serviceCategoryName: s.serviceCategoryName,
          progress: `${s.executed} / ${s.total}${s.isCompleted ? ' ✓' : ''}`,
          remaining: s.total - s.executed
        }));
        this.flatRecords = res.data.executionRecords.map(r => ({
          workerName: r.workerName,
          serviceCategoryName: r.serviceCategoryName,
          quantity: r.quantity,
          executionDate: r.executionDate ? new Date(r.executionDate).toLocaleDateString('ar-EG') : '—',
          notes: r.notes || '—'
        }));
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل سجل التنفيذ'); }
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Completed': return 'bg-success';
      case 'InProgress': return 'bg-warning text-dark';
      default: return 'bg-secondary';
    }
  }

  onBack(): void {
    this.router.navigate([`/order/groups/${this.groupId}/items`]);
  }
}
