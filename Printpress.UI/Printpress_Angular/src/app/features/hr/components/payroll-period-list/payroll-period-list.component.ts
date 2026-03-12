import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { PayrollPeriodService } from '../../services/payroll-period.service';
import { PayrollPeriodDto } from '../../models/payroll-period.dto';
import { AlertService } from '../../../../core/services/alert.service';

@Component({
  selector: 'app-payroll-period-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatTableModule, MatIconModule, MatChipsModule],
  templateUrl: './payroll-period-list.component.html'
})
export class PayrollPeriodListComponent implements OnInit {
  periods: PayrollPeriodDto[] = [];
  columns = ['name', 'startDate', 'endDate', 'status', 'actions'];

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
      next: (res) => { this.periods = res.data; },
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
