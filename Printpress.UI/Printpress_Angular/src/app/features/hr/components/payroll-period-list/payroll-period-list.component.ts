import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { PayrollPeriodService } from '../../services/payroll-period.service';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-payroll-period-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, TableTemplateComponent],
  templateUrl: './payroll-period-list.component.html'
})
export class PayrollPeriodListComponent implements OnInit {
  rows: any[] = [];

  colDefs: TableColDefinitionModel[] = [
    { column: 'name', headerName: 'اسم الدورة' },
    { column: 'startDate', headerName: 'تاريخ البداية' },
    { column: 'endDate', headerName: 'تاريخ النهاية' },
    { column: 'status', headerName: 'الحالة' }
  ];

  constructor(
    private service: PayrollPeriodService,
    private alertService: AlertService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.service.getAll().subscribe({
      next: (res) => {
        this.rows = res.data.map(p => ({
          id: p.id,
          name: p.name,
          startDate: p.startDate ? new Date(p.startDate).toLocaleDateString('ar-EG') : '—',
          endDate: p.endDate ? new Date(p.endDate).toLocaleDateString('ar-EG') : '—',
          status: p.isClosed ? 'مغلقة' : 'مفتوحة'
        }));
      },
      error: () => { this.alertService.showError('حدث خطأ أثناء تحميل دورات الرواتب'); }
    });
  }

  onAdd(): void {
    this.router.navigate(['/hr/payroll-periods/add']);
  }

  onViewDetails(id: string): void {
    this.router.navigate(['/hr/payroll-periods', id]);
  }
}
