import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { CashAccountDto } from '../../models/cash-account.dto';
import { AlertService } from '../../../../core/services/alert.service';
import { CashAccountService } from '../../services/cash-account.service';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';

@Component({
  selector: 'app-cash-account-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, TableTemplateComponent],
  templateUrl: './cash-account-list.component.html',
})
export class CashAccountListComponent implements OnInit {
  accounts: CashAccountDto[] = [];
  totalCount = 0;

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'الرصيد', column: 'balance' },
  ];

  constructor(
    private cashAccountService: CashAccountService,
    private alertService: AlertService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadAccounts();
  }

  private loadAccounts(): void {
    this.cashAccountService.getAll().subscribe({
      next: (response) => {
        this.accounts = response.data;
        this.totalCount = response.data.length;
      },
      error: () => {
        this.alertService.showError('حدث خطأ أثناء تحميل الخزائن');
      }
    });
  }

  onView(id: string): void {
    this.router.navigate(['/general/cash-accounts/view', id]);
  }
}
