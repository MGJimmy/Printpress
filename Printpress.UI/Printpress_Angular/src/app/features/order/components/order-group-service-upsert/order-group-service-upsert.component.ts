import { Component, EventEmitter, Inject, OnDestroy, OnInit, Output } from '@angular/core';
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
import { Observable, Subscription } from 'rxjs';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { ChangeDetectorRef } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ServiceService } from '../../../setup/services/service.service';
import { ServiceGetDto } from '../../../setup/models/service-get.dto';
import { ServiceCategoryEnum } from '../../../setup/models/service-category.enum';
import { OrderSharedDataService } from '../../services/order-shared-data.service';

export interface ServiceCat_interface {
  id: number;
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

  tableData: ServiceGetDto[] | null = null;

  sellingCategories: ServiceCat_interface[] = [];
  otherCategories: ServiceCat_interface[] = [];
  serviceCategories!: string[] 
  allServices: ServiceGetDto[] = [];

  selectedCategory: string | null = null;
  selectedServiceId: number | null = null;
  isSellingSelected: boolean | null = null;
  subscriptions: Subscription = new Subscription();


  filteredServices: ServiceGetDto[] = [];
  selectedServiceCategoryId: number | null = null;

  groupId: number = 1;

  constructor(
    private alertService: AlertService,
    private dialogService: DialogService,
    private errorHandlingService: ErrorHandlingService,
    private currentComponentDialogRef: MatDialogRef<OrderGroupServiceUpsertComponent>,
    private serviceService: ServiceService,
    private orderSharedDataService:OrderSharedDataService,
    @Inject(MAT_DIALOG_DATA) public inputData: any
  ) {}

  ngOnInit(): void {
    this.groupId = this.inputData.groupId;
    console.log(this.groupId);

    this.fetchServices();
    this.fillPageData()

  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  fetchServices(): void {
    this.serviceService.getAll().subscribe({
      next: (data) => {
        this.allServices = data;
      },
      error: (err) => {
        this.errorHandlingService.handleError(err);
      },
    });
  }


  private fillPageData(){
    this.fillServiceCategoriesList();
    this.fillTableData();
    this.clearSelections();
  }

  fillServiceCategoriesList(){
    let group = this.orderSharedDataService.getOrderGroup(this.groupId);

    if(!group.orderGroupServices || group.orderGroupServices.length == 0){
      this.serviceCategories = Object.keys(ServiceCategoryEnum);
      return;
    }

    if(group.isHasSellingService){
      this.serviceCategories = Object.keys(ServiceCategoryEnum).filter(key => key === ServiceCategoryEnum.Selling);
      return;
    }
    
    this.serviceCategories = Object.keys(ServiceCategoryEnum).filter(key => key != ServiceCategoryEnum.Selling);
  }

  private fillTableData(){
    let groupServices = this.orderSharedDataService.getOrderGroupServices(this.groupId);
    
    if(!groupServices || groupServices.length == 0){
      return;
    }

    this.serviceService.getServices(groupServices.map(x => x.serviceId)).subscribe(services =>{
      this.tableData = services;
    });
  }


  // onCategorySelect_old(categoryId: number): void {
  //   const isSelling = this.serviceCategories.some((cat) => cat.id === categoryId && cat.name === 'بيع');

  //   if (this.isSellingSelected === null || this.isSellingSelected === isSelling) {
  //     this.isSellingSelected = isSelling;
  //     this.selectedCategory = categoryId;

  //     this.filteredServices = this.allServices.filter((svc) => svc.serviceCategoryId === categoryId);
  //     this.serviceCategories = isSelling ? this.sellingCategories : this.otherCategories;

  //   } else {
  //     this.alertService.showError('يمكنك اختيار فئة واحدة فقط');
  //     this.clearSelections();
  //   }
  // }

  onCategorySelect(serviceCategoryEnumValue: string): void {
    this.filteredServices = this.allServices.filter(s => s.serviceCategory === (serviceCategoryEnumValue as ServiceCategoryEnum));
  }



  clearSelections(): void {
    this.isSellingSelected = null;
    this.selectedCategory = null;
    this.selectedServiceId = null;
  }

  addGroupService(): void {
    if (!this.selectedCategory || !this.selectedServiceId) {
      this.alertService.showError('من فضلك اختر نوع الخدمة أولا');
      this.clearSelections();
      return;
    }

    const selectedService = this.allServices.find((svc) => svc.id === this.selectedServiceId);

    if (!selectedService) {
      this.alertService.showError('حدث خطأ في اختيار نوع الخدمة');
      return;
    }

    if (this.tableData?.some((row) => row.id === selectedService.id)) {
      this.alertService.showError('لا يمكنك إضافة الخدمة مرتين');
      return;
    }

    this.orderSharedDataService.addOrderGroupService(this.groupId, selectedService.id);
    this.fillPageData();

    this.alertService.showSuccess('تم إضافة الخدمة بنجاح');
  }

  onDeleteServiceCat(serviceId: number): void {
    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذه الخدمة ؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    const dialogSub = this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {
        this.orderSharedDataService.deleteGroupService(this.groupId, serviceId);
        this.fillPageData();
        this.alertService.showSuccess('تم حذف الخدمة بنجاح!');
      }
    });

    this.subscriptions.add(dialogSub);
  }

  protected Save(){
    this.currentComponentDialogRef.close();
  }
}
