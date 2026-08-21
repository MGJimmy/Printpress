import { Component, EventEmitter, Inject, OnDestroy, OnInit } from '@angular/core';
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
import { ServiceCategoryArabicPipe } from '../../../setup/Pipes/service-category-arabic.pipe';
import { TranslationService } from '../../../../core/services/translation.service';
import { isStatus } from '../../models/enums/status-display';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';

export interface ServiceCat_interface {
  id: string;
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
    CommonModule,
    MatDialogModule,
    ServiceCategoryArabicPipe,
    MatRadioModule,
    MatCheckboxModule
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

  sellingCategories: ServiceCat_interface[] = [];
  otherCategories: ServiceCat_interface[] = [];
  serviceCategories!: ServiceCategoryEnum[]
  allServices: ServiceGetDto[] = [];

  selectedCategory: string | null = null;
  selectedServiceId: string | null = null;
  isCover = false;
  isCoverCheckboxDisabled = false;
  isSellingSelected: boolean | null = null;
  subscriptions: Subscription = new Subscription();


  filteredServices: ServiceGetDto[] = [];
  selectedServiceCategoryId: string | null = null;

  groupId: string = '';
  servicesLocked = false;

  executionType: string = 'Internal';
  executionTypes = ['Internal', 'External_WithOurMaterials', 'External_Full'];
  executionTypeLabels: Record<string, string> = {
    Internal: 'داخلي',
    External_WithOurMaterials: 'خارجي (بموادنا)',
    External_Full: 'خارجي (كامل)'
  };

  get isPrintingCategorySelected(): boolean {
    return this.selectedCategory === ServiceCategoryEnum.Printing;
  }

  constructor(
    private alertService: AlertService,
    private dialogService: DialogService,
    private errorHandlingService: ErrorHandlingService,
    private currentComponentDialogRef: MatDialogRef<OrderGroupServiceUpsertComponent>,
    private serviceService: ServiceService,
    private orderSharedDataService:OrderSharedDataService,
    @Inject(MAT_DIALOG_DATA) public inputData: any,
    private _t: TranslationService
  ) {}

  ngOnInit(): void {
    this.groupId = this.inputData.groupId;

    const group = this.orderSharedDataService.getOrderGroup_Copy(this.groupId);
    this.executionType = group.executionType ?? 'Internal';
    this.servicesLocked = isStatus(group.status, 'Completed', 'Delivered')
      || !!group.deliveryDate
      || (group.items ?? []).some(item => item.hasExecutions === true || isStatus(item.status, 'Completed'));

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
        this.allServices = this.allServices.filter(s => s.isActive !== false);
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
    let group = this.orderSharedDataService.getOrderGroup_Copy(this.groupId);

    if(!group.orderGroupServices || group.orderGroupServices.length == 0){
      this.serviceCategories =  Object.keys(ServiceCategoryEnum).sort() as ServiceCategoryEnum[];
      return;
    }

    if(group.isHasSellingService){
      this.serviceCategories = Object.keys(ServiceCategoryEnum).filter(key => key === ServiceCategoryEnum.Selling).sort() as ServiceCategoryEnum[];
      return;
    }

    this.serviceCategories = Object.keys(ServiceCategoryEnum).filter(key => key != ServiceCategoryEnum.Selling).sort() as ServiceCategoryEnum[];
  }


  private fillTableData(){
    let groupServices = this.orderSharedDataService.getOrderGroupServices_copy(this.groupId);

    if(!groupServices || groupServices.length == 0){
      this.tableData = [];
      return;
    }

    this.serviceService.getServices(groupServices.map(x => x.serviceId)).subscribe(services =>{
      this.tableData = services.map(service => {
        const groupService = groupServices.find(gs => gs.serviceId === service.id);
        return {
          ...service,
          name: groupService?.isCover ? `غلاف ${service.name}` : service.name
        };
      });
    });
  }

  onCategorySelect(serviceCategoryEnumValue: string): void {
    this.filteredServices = this.allServices.filter(s => s.serviceCategoryCode === serviceCategoryEnumValue);
    this.selectedServiceId = null;
    this.updateCoverCheckboxState();
  }

  private updateCoverCheckboxState(): void {
    if (!this.isPrintingCategorySelected) {
      this.isCover = false;
      this.isCoverCheckboxDisabled = false;
      return;
    }

    const hasCover = this.hasCoverPrintingService();
    this.isCoverCheckboxDisabled = hasCover;
    if (hasCover) {
      this.isCover = false;
    }
  }

  private hasCoverPrintingService(): boolean {
    const groupServices = this.orderSharedDataService.getOrderGroupServices_copy(this.groupId);
    return groupServices.some(gs => gs.isCover === true);
  }

  private hasMainPrintingService(): boolean {
    const groupServices = this.orderSharedDataService.getOrderGroupServices_copy(this.groupId);
    return groupServices.some(gs => {
      const service = this.allServices.find(s => s.id === gs.serviceId)
        ?? this.tableData?.find(s => s.id === gs.serviceId);
      return service?.serviceCategoryCode === ServiceCategoryEnum.Printing && gs.isCover !== true;
    });
  }

  clearSelections(): void {
    this.isSellingSelected = null;
    this.selectedCategory = null;
    this.selectedServiceId = null;
    this.isCover = false;
    this.isCoverCheckboxDisabled = false;
    this.filteredServices = [];
  }

  addGroupService(): void {
    if (this.servicesLocked) {
      this.alertService.showError(this._t.t('orders.cannot_change_services_after_execution'));
      return;
    }
    if (!this.selectedCategory || !this.selectedServiceId) {
      this.alertService.showError(this._t.t('orders.select_service_type_first'));
      this.clearSelections();
      return;
    }

    const selectedService = this.allServices.find((svc) => svc.id === this.selectedServiceId);

    if (!selectedService) {
      this.alertService.showError(this._t.t('orders.error_service_type'));
      return;
    }

    if (selectedService.serviceCategoryCode === ServiceCategoryEnum.Printing) {
      if (this.isCover && this.hasCoverPrintingService()) {
        this.alertService.showError(this._t.t('orders.printing_cover_duplicate'));
        return;
      }

      if (!this.isCover && this.hasMainPrintingService()) {
        this.alertService.showError(this._t.t('orders.printing_main_duplicate'));
        return;
      }
    } else if (this.tableData?.some((row) => row.serviceCategoryCode === selectedService.serviceCategoryCode)) {
      this.alertService.showError(this._t.t('orders.service_type_duplicate'));
      return;
    }

    const isCover = selectedService.serviceCategoryCode === ServiceCategoryEnum.Printing && this.isCover;
    this.orderSharedDataService.addOrderGroupService(this.groupId, selectedService, isCover);
    this.fillPageData();

    this.alertService.showSuccess(this._t.t('orders.service_added'));
  }

  protected onDeleteServiceCat(serviceId: string): void {
    if (this.servicesLocked) {
      this.alertService.showError(this._t.t('orders.cannot_change_services_after_execution'));
      return;
    }
    if (!this.validateBeforeDelete()) {
      this.alertService.showError(this._t.t('orders.service_delete_has_items'));
      return;
    }

    const dialogData: ConfirmDialogModel = {
      title: this._t.t('orders.confirm_delete'),
      message: this._t.t('orders.delete_service_msg'),
      confirmText: this._t.t('shared.yes'),
      cancelText: this._t.t('shared.cancel'),
    };

    const dialogSub = this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {
        this.orderSharedDataService.deleteGroupService(this.groupId, serviceId);
        this.fillPageData();
        this.alertService.showSuccess(this._t.t('orders.service_deleted'));
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

  onExecutionTypeChange(type: string): void {
    if (this.servicesLocked) {
      return;
    }
    this.executionType = type;
    this.orderSharedDataService.updateGroupExecutionType(this.groupId, type);
  }

  onCancel(): void {
    if (this.inputData?.deleteGroupOnCancel) {
      this.orderSharedDataService.discardTempGroup(this.groupId);
    }

    this.currentComponentDialogRef.close(false);
  }

  protected onClickSave() {
    if (!this.tableData || this.tableData.length == 0) {
      this.alertService.showError(this._t.t('orders.services_empty'));
      return;
    }

    this.currentComponentDialogRef.close(true);

  }
}