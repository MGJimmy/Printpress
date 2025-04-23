import { Component, OnInit } from '@angular/core';
import { ServiceGetDto } from '../../models/service-get.dto';
import { ServiceService } from '../../services/service.service';
import { MatDialog } from '@angular/material/dialog';
import { ServiceUpsertComponent } from '../service-upsert/service-upsert.component';
import { AlertService } from '../../../../core/services/alert.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { finalize, map } from 'rxjs';

@Component({
  selector: 'app-service-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatCardModule,
    CommonModule,
    TableTemplateComponent
  ],
  templateUrl: './service-list.component.html',
  styleUrls: ['./service-list.component.css']
})
export class ServiceListComponent implements OnInit {
  isLoading = false;
  services: ServiceGetDto[] = [];
  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'الاسم', column: 'name' },
    { headerName: 'السعر', column: 'price' },
    { headerName: 'نوع الخدمة', column: 'serviceCategoryName' }
  ];

  constructor(
    private serviceService: ServiceService,
    private dialog: MatDialog,
    private alertService: AlertService
  ) { }

  ngOnInit() {
    this.loadServices();
  }

  loadServices() {
    this.isLoading = true;
    this.serviceService.getAll()
      .pipe(
        map(services => services.map(service => ({
          ...service,
          serviceCategoryName: service.serviceCategory
        }))),
        finalize(() => this.isLoading = false)
      )
      .subscribe({
        next: (services) => {
          this.services = services;
        },
        error: (error) => {
          this.alertService.showError('حدث خطأ أثناء تحميل الخدمات');
        }
      });
  }

  openServiceDialog(id?: number) {
    const service = id ? this.services.find(s => s.id === id) : undefined;
    const dialogRef = this.dialog.open(ServiceUpsertComponent, {
      width: '500px',
      data: service
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadServices();
      }
    });
  }

  onEdit(id: number) {
    this.openServiceDialog(id);
  }

  onDelete(id: number) {
    if (confirm('هل أنت متأكد من حذف هذه الخدمة؟')) {
      this.isLoading = true;
      this.serviceService.delete(id)
        .pipe(finalize(() => this.isLoading = false))
        .subscribe({
          next: () => {
            this.alertService.showSuccess('تم حذف الخدمة بنجاح');
            this.loadServices();
          },
          error: (error) => {
            this.alertService.showError('حدث خطأ أثناء حذف الخدمة');
          }
        });
    }
  }
}
