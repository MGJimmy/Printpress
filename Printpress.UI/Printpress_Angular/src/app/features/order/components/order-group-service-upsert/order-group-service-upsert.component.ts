import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
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
import { Subscription } from 'rxjs';
import { ErrorHandlingService } from '../../../../core/helpers/error-handling.service';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { ServiceService } from '../../../setup/services/service.service';
import { ServiceGetDto } from '../../../setup/models/service-get.dto';
import { ServiceCategoryEnum } from '../../../setup/models/service-category.enum';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { ServiceCategoryService } from '../../../setup/services/service-category.service';
import { ServiceCategoryDto } from '../../../setup/models/service-category.dto';
import { InventoryService } from '../../../inventory/services/inventory.service';
import { InventoryItemDto } from '../../../inventory/models/inventory-item.dto';

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
    CommonModule,
    MatDialogModule,
  ],
  templateUrl: './order-group-service-upsert.component.html',
  styleUrl: './order-group-service-upsert.component.css'
})

export class OrderGroupServiceUpsertComponent implements OnInit, OnDestroy {

  columnDefs: TableColDefinitionModel[] = [
    { headerName: 'مسلسل', column: 'id' },
    { headerName: 'الخدمة', column: 'name' }
  ];

  tableData: ServiceGetDto[] | null = null;

  allServiceCategories: ServiceCategoryDto[] = [];
  serviceCategories: ServiceCategoryDto[] = [];
  allServices: ServiceGetDto[] = [];

  selectedCategoryId: string | null = null;
  selectedServiceId: string | null = null;
  selectedInventoryItemId: string | null = null;
  subscriptions: Subscription = new Subscription();

  filteredServices: ServiceGetDto[] = [];
  filteredInventoryItems: InventoryItemDto[] = [];
  requiresInventoryItem: boolean = false;

  groupId: string = '';

  constructor(
    private alertService: AlertService,
    private dialogService: DialogService,
    private errorHandlingService: ErrorHandlingService,
    private currentComponentDialogRef: MatDialogRef<OrderGroupServiceUpsertComponent>,
    private serviceService: ServiceService,
    private serviceCategoryService: ServiceCategoryService,
    private inventoryService: InventoryService,
    private orderSharedDataService: OrderSharedDataService,
    @Inject(MAT_DIALOG_DATA) public inputData: any
  ) {}

  ngOnInit(): void {
    this.groupId = this.inputData.groupId;

    this.serviceCategoryService.getAll().subscribe({
      next: (categories) => {
        this.allServiceCategories = categories;
        this.fillPageData();
      },
      error: (err) => this.errorHandlingService.handleError(err)
    });

    this.serviceService.getAll().subscribe({
      next: (data) => { this.allServices = data; },
      error: (err) => this.errorHandlingService.handleError(err)
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private fillPageData(): void {
    this.fillServiceCategoriesList();
    this.fillTableData();
    this.clearSelections();
  }

  private fillServiceCategoriesList(): void {
    const group = this.orderSharedDataService.getOrderGroup_Copy(this.groupId);

    if (!group.orderGroupServices || group.orderGroupServices.length === 0) {
      this.serviceCategories = [...this.allServiceCategories].sort((a, b) => a.name.localeCompare(b.name));
      return;
    }

    if (group.isHasSellingService) {
      this.serviceCategories = this.allServiceCategories.filter(c => c.code === ServiceCategoryEnum.Selling);
      return;
    }

    this.serviceCategories = this.allServiceCategories
      .filter(c => c.code !== ServiceCategoryEnum.Selling)
      .sort((a, b) => a.name.localeCompare(b.name));
  }

  private fillTableData(): void {
    const groupServices = this.orderSharedDataService.getOrderGroupServices_copy(this.groupId);

    if (!groupServices || groupServices.length === 0) {
      this.tableData = [];
      return;
    }

    this.serviceService.getServices(groupServices.map(x => x.serviceId)).subscribe(services => {
      this.tableData = services;
    });
  }

  onCategorySelect(categoryId: string): void {
    const category = this.allServiceCategories.find(c => c.id === categoryId);

    this.selectedServiceId = null;
    this.selectedInventoryItemId = null;
    this.filteredInventoryItems = [];
    this.requiresInventoryItem = false;

    if (!category) return;

    this.filteredServices = this.allServices.filter(s => s.serviceCategoryCode === category.code);

    if (category.requireInventoryItem && category.inventoryItemCategoryId != null) {
      this.requiresInventoryItem = true;
      this.inventoryService.getByCategory(category.inventoryItemCategoryId).subscribe({
        next: (res) => { this.filteredInventoryItems = res.data; },
        error: (err) => this.errorHandlingService.handleError(err)
      });
    }
  }

  clearSelections(): void {
    this.selectedCategoryId = null;
    this.selectedServiceId = null;
    this.selectedInventoryItemId = null;
    this.filteredServices = [];
    this.filteredInventoryItems = [];
    this.requiresInventoryItem = false;
  }

  addGroupService(): void {
    if (!this.selectedCategoryId || !this.selectedServiceId) {
      this.alertService.showError('من فضلك اختر نوع الخدمة أولا');
      this.clearSelections();
      return;
    }

    if (this.requiresInventoryItem && !this.selectedInventoryItemId) {
      this.alertService.showError('من فضلك اختر عنصر المخزون');
      return;
    }

    const selectedService = this.allServices.find((svc) => svc.id === this.selectedServiceId);

    if (!selectedService) {
      this.alertService.showError('حدث خطأ في اختيار نوع الخدمة');
      return;
    }

    if (this.tableData?.some((row) => row.serviceCategoryCode === selectedService.serviceCategoryCode)) {
      this.alertService.showError('لا يمكنك إضافة خدمات من نفس النوع أكثر من مرة');
      return;
    }

    this.orderSharedDataService.addOrderGroupService(this.groupId, selectedService);
    this.fillPageData();

    this.alertService.showSuccess('تم إضافة الخدمة بنجاح');
  }

  protected onDeleteServiceCat(serviceId: string): void {
    if (!this.validateBeforeDelete()) {
      this.alertService.showError('لا يمكن حذف خدمة. المجموعة تحتوي علي عناصر مضافة');
      return;
    }
    
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
  private validateBeforeDelete(){
    const items = this.orderSharedDataService.getOrderGroupItems_copy(this.groupId)
    if (items && items.length > 0) {
      return false;
    }

    return true;
  }

  protected onClickSave() {
    if (!this.tableData || this.tableData.length == 0) {
      this.alertService.showError("لا يمكن حفظ خدمات المجموعة فارغة");
      return;
    }

    this.currentComponentDialogRef.close(true);

  }
}
