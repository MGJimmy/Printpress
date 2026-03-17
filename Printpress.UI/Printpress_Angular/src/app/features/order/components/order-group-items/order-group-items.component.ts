import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { ItemServiceExecutionService } from '../../services/item-service-execution.service';
import {
  OrderGroupItemsResponseDto,
  ItemWithServiceProgressDto,
  ServiceProgressDto,
  GroupStatusLabels,
  ItemStatusLabels
} from '../../models/execution/execution.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-order-group-items',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatBadgeModule,
    MatMenuModule
  ],
  templateUrl: './order-group-items.component.html'
})
export class OrderGroupItemsComponent implements OnInit {
  groupId!: string;
  groupData: OrderGroupItemsResponseDto | null = null;
  filteredItems: ItemWithServiceProgressDto[] = [];
  statusFilter: string = 'all';
  displayedColumns: string[] = [];

  groupStatusLabels = GroupStatusLabels;
  itemStatusLabels = ItemStatusLabels;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private executionService: ItemServiceExecutionService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.groupId = this.route.snapshot.paramMap.get('groupId')!;
    this.loadGroupItems();
  }

  private loadGroupItems(): void {
    this.executionService.getGroupItems(this.groupId).subscribe({
      next: (res) => {
        this.groupData = res.data;
        this.buildColumns();
        this.applyFilter();
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل بيانات المجموعة'); }
    });
  }

  private buildColumns(): void {
    const serviceColumns = (this.groupData?.groupServices ?? []).map(s => `svc_${s.serviceCategoryId}`);
    this.displayedColumns = ['name', 'quantity', 'status', ...serviceColumns, 'actions'];
  }

  applyFilter(): void {
    if (!this.groupData) return;

    switch (this.statusFilter) {
      case 'completed':
        this.filteredItems = this.groupData.items.filter(i => i.status === 'Completed');
        break;
      case 'notCompleted':
        this.filteredItems = this.groupData.items.filter(i => i.status !== 'Completed');
        break;
      default:
        this.filteredItems = [...this.groupData.items];
    }
  }

  getServiceProgress(item: ItemWithServiceProgressDto, serviceCategoryId: string): ServiceProgressDto | null {
    return item.serviceProgresses.find(s => s.serviceCategoryId === serviceCategoryId) ?? null;
  }

  onCompleteItem(itemId: string): void {
    this.executionService.completeItem(itemId).subscribe({
      next: () => {
        this.alertService.showSuccess('تم اكتمال العنصر');
        this.loadGroupItems();
      },
      error: () => this.alertService.showError('حدث خطأ أثناء اكتمال العنصر')
    });
  }

  onExecuteItem(itemId: string): void {
    this.router.navigate([`/order/groups/${this.groupId}/items/${itemId}/execute`]);
  }

  onViewItem(itemId: string): void {
    this.router.navigate([`/order/groups/${this.groupId}/items/${itemId}/history`]);
  }

  onBack(): void {
    this.router.navigate(['/orderlist']);
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Completed': return 'badge-completed';
      case 'InProgress': return 'badge-inprogress';
      default: return 'badge-new';
    }
  }

  getGroupStatusBadgeClass(): string {
    return this.getStatusBadgeClass(this.groupData?.groupStatus ?? '');
  }
}
