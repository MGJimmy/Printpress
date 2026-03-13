import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ItemServiceExecutionService } from '../../services/item-service-execution.service';
import {
  ItemExecutionHistoryDto,
  ItemExecutionRecordDto,
  ServiceProgressDto,
  ItemStatusLabels
} from '../../models/execution/execution.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-item-execution-history',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './item-execution-history.component.html'
})
export class ItemExecutionHistoryComponent implements OnInit {
  itemId!: string;
  groupId!: string;
  history: ItemExecutionHistoryDto | null = null;

  itemStatusLabels = ItemStatusLabels;
  recordColumns = ['workerName', 'serviceCategoryName', 'quantity', 'executionDate', 'notes'];
  progressColumns = ['serviceCategoryName', 'executed', 'remaining'];

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
      next: (res) => { this.history = res.data; },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل سجل التنفيذ'); }
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Completed': return 'badge-success';
      case 'InProgress': return 'badge-warning';
      default: return 'badge-secondary';
    }
  }

  getProgressClass(svc: ServiceProgressDto): string {
    return svc.isCompleted ? 'text-success fw-bold' : 'text-warning';
  }

  onBack(): void {
    this.router.navigate([`/order/groups/${this.groupId}/items`]);
  }
}
