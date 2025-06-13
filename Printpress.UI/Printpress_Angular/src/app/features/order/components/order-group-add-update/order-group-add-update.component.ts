import { Component, Injector, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PageEvent } from '@angular/material/paginator';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { CommonModule } from '@angular/common';
import { AlertService } from '../../../../core/services/alert.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog'
import { OrderGroupServiceUpsertComponent } from '../order-group-service-upsert/order-group-service-upsert.component';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderGroupServiceGetDto } from '../../models/orderGroupService/order-group-service-get.Dto';
import { ItemGetDto } from '../../models/item/item-get.Dto';
import { ObjectStateEnum } from '../../../../core/models/object-state.enum';
import { ConfirmDialogModel } from '../../../../core/models/confirm-dialog.model';
import { ItemGridVM } from '../../models/item/itemGridVM';
import { itemDetailsKeyEnum } from '../../models/enums/item-details-key.enum';
import { DialogService } from '../../../../shared/services/dialog.service';
import { OrderGroupGetDto } from '../../models/orderGroup/order-group-get.Dto';
import { OrderRoutingService } from '../../services/order-routing.service';

@Component({
  selector: 'app-order-group-add-update',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule, SharedPaginationComponent,
    CommonModule, MatDialogModule],
  templateUrl: './order-group-add-update.component.html',
  styleUrl: './order-group-add-update.component.css'
})
export class OrderGroupAddUpdateComponent implements OnInit {

  private groupId!: number;

  protected isEdit: boolean = false;
  protected groupName: string = '';
  protected groupItems: ItemGetDto[] = [];

  protected itemsGridSource!: ItemGridVM[];

  protected groupServicesNamesCommaseperated!: string;

  private updateDisplayedServicesNames(groupServices: OrderGroupServiceGetDto[]) {
    this.groupServicesNamesCommaseperated = groupServices.map(x => { return x.serviceName }).join(' - ');
  }

  protected displayedColumns: string[] = [];

  constructor(
    private alertService: AlertService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private orderSharedService: OrderSharedDataService,
    private injector: Injector,
    private dialogService: DialogService,
    private orderRoutingService: OrderRoutingService
  ) {
  }

  ngOnInit(): void {
    this.setGroupId();

    this.setIsEdit();
    this.setCurrentGroupData();
    this.updateDisplayedColumns();

    if (!this.groupItems || this.groupItems.length == 0) {
      this.openServicesModal();
    }
  }

  private setGroupId(): void {
    const param_GroupId = this.route.snapshot.paramMap.get('id');
    if (param_GroupId) {
      this.groupId = Number(param_GroupId);
    } else {
      this.groupId = this.orderSharedService.intializeNewGroup();
    }
  }

  private setIsEdit() {

    const currentGroup = this.orderSharedService.getOrderGroup_Copy(this.groupId);

    if (currentGroup.objectState == ObjectStateEnum.temp || currentGroup.objectState == ObjectStateEnum.added) {
      this.isEdit = false;
    } else {
      this.isEdit = true;
    }
  }

  private setCurrentGroupData() {
    const currentGroup = this.orderSharedService.getOrderGroup_Copy(this.groupId);

    this.groupName = currentGroup.name;
    this.updateDisplayedServicesNames(currentGroup.orderGroupServices);

    this.groupItems = currentGroup.items;
    this.mapItemsGrid(this.groupItems);
  }

  protected updateDisplayedColumns() {
    const group = this.orderSharedService.getOrderGroup_Copy(this.groupId);
    let allColumns = [
      { key: 'index', condition: () => true },
      { key: 'name', condition: () => true },
      { key: 'quantity', condition: () => true },
      { key: 'itemPrice', condition: () => false },
      { key: 'numberOfPages', condition: () => group.isHasPrintingService },
      { key: 'printedItemsCount', condition: () => group.isHasPrintingService },
      { key: 'stapledItemsCount', condition: () => group.isHasStaplingService },
      { key: 'boughtItemsCount', condition: () => group.isHasSellingService },
      { key: 'total', condition: () => false },
      { key: 'actions', condition: () => true }
    ];

    this.displayedColumns = allColumns.filter(x => x.condition()).map(x => x.key);
  }

  protected groupNameChanged() {
    this.orderSharedService.updateOrderGroupName(this.groupId, this.groupName);
  }

  protected editGroupService_Click(): void {
    this.openServicesModal();
  }

  private openServicesModal() {
    let dialogRef = this.dialog.open(OrderGroupServiceUpsertComponent, {
      data: { groupId: this.groupId },
      width: '1000px',
      disableClose: true,
      injector: this.injector
    });

    dialogRef.afterClosed().subscribe((isSave: boolean) => {
      if (!isSave) {
        return;
      }
      const groupServices = this.orderSharedService.getOrderGroupServices_copy(this.groupId);
      this.updateDisplayedServicesNames(groupServices);
      this.updateDisplayedColumns();
    });
  }

  protected getDetailValueByKeyName(itemId: number, key: string) {
    const item: ItemGetDto = this.orderSharedService.getItem_copy(this.groupId, itemId);
    item.details.find(x => x.key == key)?.value;
  }

  protected addItem_Click() {
    this.router.navigate([this.orderRoutingService.getItemAddRoute(this.groupId)]);
  }

  protected editItem_Click(item: ItemGetDto) {
    this.router.navigate([this.orderRoutingService.getItemEditRoute(this.groupId, item.id)]);
  }

  protected deleteItem_Click(item: ItemGetDto) {
    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذا العنصر ؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {
        this.orderSharedService.deleteItem(this.groupId, item.id);

        this.setCurrentGroupData();

        this.alertService.showSuccess('تم حذف العنصر بنجاح!');
      }
    });
  }

  protected onPageChangeClick(event: PageEvent) {
    const length = event.pageSize;
    const pageNumber = event.pageIndex;
    const itemsToDisplay = this.groupItems.slice((pageNumber) * length, (pageNumber + 1) * length);

    this.mapItemsGrid(itemsToDisplay);
  }

  protected onSave_Click() {
    if (!this.validateGroup()) {
      return;
    }

    if (this.isEdit) {
      this.orderSharedService.updateOrderGroup(this.groupId);
    } else {
      this.orderSharedService.addOrderGroup(this.groupId);
    }

    this.navigateToOrderPage();
  }

  private validateGroup(): boolean {
    if (!this.groupName) {
      this.alertService.showError('أدخل اسم المجموعة');
      return false;
    }

    if (!this.groupItems || this.groupItems.length == 0) {
      this.alertService.showError('لابد من إدخال عناصر للمجموعة');
      return false;
    }

    return true;
  }

  protected async onBack_Click() {

    if (! await this.dialogService.confirmOnBackButton()) {
      return;
    }

    if (!this.isEdit) {
      this.orderSharedService.deleteGroup(this.groupId);
    }

    this.navigateToOrderPage();
  }

  private navigateToOrderPage() {
    if (this.orderSharedService.getOrderObject_copy().objectState == ObjectStateEnum.temp) {
      this.router.navigate([this.orderRoutingService.getOrderAddRoute()]);
    } else {
      this.router.navigate([this.orderRoutingService.getOrderEditRoute(this.orderSharedService.getOrderObject_copy().id)]);
    }
  }

  private mapItemsGrid(items: ItemGetDto[]): void {
    let itemVMList: ItemGridVM[] = [];
    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      const numberOfPages = item.details?.find(x => x.key === itemDetailsKeyEnum.NumberOfPages);
      const printedItemsCount = item.details?.find(x => x.key === itemDetailsKeyEnum.PrintedItemsCount);
      const stapledItemsCount = item.details?.find(x => x.key === itemDetailsKeyEnum.StapledItemsCount);
      const boughtItemsCount = item.details?.find(x => x.key === itemDetailsKeyEnum.BoughtItemsCount);


      let itemVM: ItemGridVM = {
        id: item.id,
        name: item.name,
        quantity: item.quantity ? item.quantity.toString() : '',
        price: item.price ? item.price.toString() : '',
        total: (item.price && item.quantity) ? (item.price * item.quantity).toString() : '',
        boughtItemsCount: boughtItemsCount?.value ?? '',
        numberOfPages: numberOfPages?.value ?? '',
        printedItemsCount: printedItemsCount?.value ?? '',
        stapledItemsCount: stapledItemsCount?.value ?? ''
      };

      itemVMList.push(itemVM);
    }
    this.itemsGridSource = itemVMList;
  }
}
