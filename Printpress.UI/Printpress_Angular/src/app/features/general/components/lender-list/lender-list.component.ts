import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { finalize } from 'rxjs';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { AlertService } from '../../../../core/services/alert.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { LenderService } from '../../services/loan.service';
import { LenderDto } from '../../models/loan.dto';

@Component({
  selector: 'app-lender-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, TableTemplateComponent],
  templateUrl: './lender-list.component.html',
  styleUrl: '../loans.shared.scss',
})
export class LenderListComponent implements OnInit {
  lenders: LenderDto[] = [];
  totalCount = 0;
  isDeleting = false;

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'الهاتف', column: 'phone' },
    { headerName: 'ملاحظات', column: 'notes' },
  ];

  constructor(
    private lenderService: LenderService,
    private alertService: AlertService,
    private dialogService: DialogService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.lenderService.getAll().subscribe({
      next: (res) => {
        this.lenders = res.data ?? [];
        this.totalCount = this.lenders.length;
      },
      error: () => this.alertService.showError('حدث خطأ أثناء تحميل المقرضين'),
    });
  }

  onAdd(): void {
    this.router.navigate(['/general/lenders/new']);
  }

  onEdit(id: string): void {
    this.router.navigate(['/general/lenders', id]);
  }

  onDelete(id: string): void {
    this.dialogService.confirmDialog({
      title: 'تأكيد الحذف',
      message: 'هل تريد حذف هذا المقرض؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    }).subscribe((ok) => {
      if (!ok || this.isDeleting) return;
      this.isDeleting = true;
      this.lenderService.delete(id).pipe(finalize(() => { this.isDeleting = false; })).subscribe({
        next: () => {
          this.alertService.showSuccess('تم حذف المقرض');
          this.load();
        },
      });
    });
  }
}
