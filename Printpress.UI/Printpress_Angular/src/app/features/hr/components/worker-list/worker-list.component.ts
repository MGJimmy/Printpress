import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { WorkerService } from '../../services/worker.service';
import { WorkerDto, SalaryTypeLabels } from '../../models/worker.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-worker-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, TableTemplateComponent],
  templateUrl: './worker-list.component.html'
})
export class WorkerListComponent implements OnInit {
  workers: WorkerDto[] = [];
  pageSize = DEFAULT_PAGE_SIZE;
  pageNumber = DEFAULT_PAGE_NUMBER;
  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'رقم الهاتف', column: 'phoneNumber' },
    { headerName: 'نوع الراتب', column: 'salaryTypeLabel' },
    { headerName: 'قيمة الراتب', column: 'salaryValue' },
    { headerName: 'الحالة', column: 'statusLabel' }
  ];

  constructor(
    private workerService: WorkerService,
    private alertService: AlertService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadWorkers();
  }

  private loadWorkers(): void {
    this.workerService.getAll(this.pageSize, this.pageNumber).subscribe({
      next: (res) => {
        this.workers = res.data.items.map(w => ({
          ...w,
          salaryTypeLabel: SalaryTypeLabels[w.salaryType] ?? '',
          salaryValue: w.salaryType === 1 ? (w.monthlySalary ?? 0) : (w.dailySalary ?? 0),
          statusLabel: w.isActive ? 'نشط' : 'غير نشط'
        } as any));
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل العمال'); }
    });
  }

  onAdd(): void {
    this.router.navigate(['/hr/workers/add']);
  }

  onEdit(id: string): void {
    this.router.navigate(['/hr/workers/edit', id]);
  }

  onView(id: string): void {
    this.router.navigate(['/hr/workers', id]);
  }

  onDeactivate(id: string): void {
    if (!confirm('هل أنت متأكد من إلغاء تفعيل هذا العامل؟')) return;
    this.workerService.deactivate(id).subscribe({
      next: () => {
        this.alertService.showSuccess('تم إلغاء تفعيل العامل بنجاح');
        this.loadWorkers();
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'حدث خطأ أثناء إلغاء التفعيل';
        this.alertService.showError(msg);
      }
    });
  }

  onActivate(id: string): void {
    if (!confirm('هل أنت متأكد من تفعيل هذا العامل؟')) return;
    this.workerService.activate(id).subscribe({
      next: () => {
        this.alertService.showSuccess('تم تفعيل العامل بنجاح');
        this.loadWorkers();
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'حدث خطأ أثناء التفعيل';
        this.alertService.showError(msg);
      }
    });
  }
  onPageChange(event: { pageSize: number; currentPage: number }): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.currentPage;
    this.loadWorkers();
  }
}
