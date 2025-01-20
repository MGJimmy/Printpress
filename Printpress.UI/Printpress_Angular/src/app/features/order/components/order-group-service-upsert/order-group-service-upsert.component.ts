import { Component, EventEmitter, Inject, OnDestroy, OnInit, Output } from '@angular/core';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.Dto';
import { MatButtonModule } from '@angular/material/button';
import { ConfirmDialogModel } from '../../../../core/models/confirm-dialog.model';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AlertService } from '../../../../core/services/alert.service';
import { DialogService } from '../../../../shared/services/dialog.service';
import { OrderGroupMockService } from '../../services/order-group-mock.service';
import { Observable, Subscription } from 'rxjs';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { SelectedServicesMockService } from '../../services/selected-services-mock.service';
import { ChangeDetectorRef } from '@angular/core';

export interface ServiceCat_interface {
  id: number;
  name: string;
}

export interface Service_interface {
  id: number;
  serviceCategoryId: number;
  name: string;
}
@Component({
  selector: 'app-order-group-service-add-update',
  standalone: true,
  imports: [
    MatButtonModule,
    TableTemplateComponent,
    MatSelectModule,
    MatInputModule,
    MatFormField,
    MatCardModule,
    FormsModule,
    CommonModule
  ],
  templateUrl: './order-group-service-upsert.component.html',
  styleUrl: './order-group-service-upsert.component.css'
})

export class OrderGroupServiceUpsertComponent implements OnInit, OnDestroy {

  // @Output() SaveOrderGroupService = new EventEmitter<OrderGroupServiceUpsertDto>();
  // outputObject: any = { y: 15 };
  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'مسلسل', column: 'id' },
    { headerName: 'الخدمة', column: 'name' },
    { headerName: 'إجراء', column: 'action' },
  ];

  tableData: Service_interface[] = [];
  sellingCategories: ServiceCat_interface[] = [];
  otherCategories: ServiceCat_interface[] = [];
  categories: ServiceCat_interface[] = [];
  services: Service_interface[] = [];

  selectedCategoryId: number | null = null;
  selectedServiceId: number | null = null;
  isSellingSelected: boolean | null = null;
  subscriptions: Subscription = new Subscription();


  filteredServices: Service_interface[] = [];
  selectedServiceCategoryId: number | null = null;

  constructor(
    private alertService: AlertService,
    private dialogService: DialogService,
    private mockService: OrderGroupMockService,
    private errorHandlingService: ErrorHandlingService,
    private SelectedServicesMockService: SelectedServicesMockService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.fetchServiceCats();
    this.fetchServices();
    this.fetchTableData();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  fetchServiceCats(): void {
    this.mockService.getServiceCats().subscribe({
      next: (data) => {
        this.categories = data;
        this.sellingCategories = data.filter((cat) => cat.name === 'selling');
        this.otherCategories = data.filter((cat) => cat.name !== 'selling');
      },
      error: (err) => {
        this.errorHandlingService.handleError(err);
      },
    });
  }

  fetchServices(): void {
    this.mockService.getServices().subscribe({
      next: (data) => {
        this.services = data;
      },
      error: (err) => {
        this.errorHandlingService.handleError(err);
      },
    });
  }

  fetchTableData(){
    this.SelectedServicesMockService.getServices().subscribe({
      next: (data) => {this.tableData = data},
      error: (err) => {this.errorHandlingService.handleError(err)}
    })
  }

  onCategorySelect(categoryId: number): void {
    const isSelling = this.categories.some((cat) => cat.id === categoryId && cat.name === 'selling');

    if (this.isSellingSelected === null || this.isSellingSelected === isSelling) {
      this.isSellingSelected = isSelling;
      this.selectedCategoryId = categoryId;

      this.filteredServices = this.services.filter((svc) => svc.serviceCategoryId === categoryId);
      this.categories = isSelling ? this.sellingCategories : this.otherCategories;
    } else {
      this.alertService.showError('You can only select either Selling or Other Categories.');
      this.clearSelections();
    }
  }

  clearSelections(): void {
    this.isSellingSelected = null;
    this.selectedCategoryId = null;
    this.selectedServiceId = null;
  }

  addSelectedServiceToTable(): void {
    if (!this.selectedCategoryId || !this.selectedServiceId) {
      this.alertService.showError('Please select a category and a service.');
      this.clearSelections();
      return;
    }

    const selectedService = this.services.find((svc) => svc.id === this.selectedServiceId);

    if (!selectedService) {
      this.alertService.showError('Invalid service selected.');
      return;
    }

    if (this.tableData.some((row) => row.id === selectedService.id)) {
      this.alertService.showError('This service has already been added.');
      return;
    }

    this.SelectedServicesMockService.addService(selectedService).subscribe({
      next: (updatedService) => {
        this.tableData = [...this.tableData, updatedService];
        this.alertService.showSuccess('Service added successfully.');
        console.log(this.tableData);
        this.selectedServiceId = null;
      },
      error: (error) => {
        this.errorHandlingService.handleError(error);
        this.alertService.showError('Error while adding the service.');
      },
    });
  }

  onDeleteServiceCat(id: number): void {
    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذه الخدمة ؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    const dialogSub = this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {
        const deleteSub = this.SelectedServicesMockService.deleteService(id).subscribe({
          next: () => {
            this.fetchTableData();
            this.alertService.showSuccess('تم حذف الخدمة بنجاح!');
          },
          error: (err) => {
            this.alertService.showError('حدث خطأ أثناء حذف الخدمة. يرجى المحاولة مرة أخرى.');
            this.errorHandlingService.handleError(err);
          },
        });
        this.subscriptions.add(deleteSub);
      }
    });

    this.subscriptions.add(dialogSub);
  }
}
