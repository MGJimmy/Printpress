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
import { ServiceService } from '../../../setup/services/service.service';
import { ServiceCategoryEnum } from '../../../setup/models/service-category.enum';
import { ConfirmDialogModel } from '../../../../core/models/confirm-dialog.model';
import { ItemSharedVM, ItemNonSellingVM, ItemSellingVM } from '../../models/item/itemGridVM';
import { itemDetailsKeyEnum } from '../../models/enums/item-details-key.enum';

@Component({
  selector: 'app-order-group-add-update',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule, SharedPaginationComponent,
    CommonModule, MatDialogModule, OrderGroupServiceUpsertComponent],
  templateUrl: './order-group-add-update.component.html',
  styleUrl: './order-group-add-update.component.css'
})
export class OrderGroupAddUpdateComponent implements OnInit {

  private groupId!: number;

  protected isEdit: boolean = false;
  protected groupName: string = '';
  protected groupItems: ItemGetDto[] = [];
  protected groupServices: OrderGroupServiceGetDto[] = [];
  protected itemsGridSource!: ItemSharedVM[];

  protected get groupServicesNamesCommaseperated(): string {
    return this.groupServices.map(x => { return x.serviceName }).join(' - ');
  }

  protected isGroupHasSellingService: boolean = false;

  displayedColumns: string[] = [];

  updateDisplayedColumns(){
    if (this.isGroupHasSellingService) {
      this.displayedColumns =  ['index', 'name', 'quantity',
        'itemPrice', 'boughtItemsCount', 'total', 'actions'];
    } else
    {
      this.displayedColumns =   ['index', 'name',
        'numberOfPages', 'stapledItemsCount', 'printedItemsCount',
        'quantity', 'itemPrice', 'total', 'actions'];
    }
  }
  constructor(private alertService: AlertService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private orderSharedService: OrderSharedDataService,
    private serviceService: ServiceService,
    private injector: Injector
  ) {
  }

  ngOnInit(): void {
    this.setGroupId();
    this.setCurrentGroupData();
    this.orderSharedService.updateGroupFlagsOnServicesCategoriesById(this.groupId);
    // this.itemsGridSource = this.groupItems.slice(0, 5);
    this.updateDisplayedColumns();

    if (!this.isEdit) {
      this.openServicesModal();
    }
  }

  private setGroupId(): void {
    const param_GroupId = this.route.snapshot.paramMap.get('id');
    if (param_GroupId) {
      this.isEdit = true;
      this.groupId = Number(param_GroupId);
    } else {
      this.isEdit = false;
      this.groupId = this.orderSharedService.intializeNewGroup();
    }
  }

  private setCurrentGroupData() {
    const currentGroup = this.orderSharedService.getOrderGroup(this.groupId);

    this.groupName = currentGroup.name;
    // this.groupItems = currentGroup.items;
    this.groupServices = currentGroup.orderGroupServices;
    this.groupName = currentGroup.name;
  }

  protected groupNameChanged() {
    const currentGroup = this.orderSharedService.getOrderGroup(this.groupId);
    currentGroup.name = this.groupName;
  }

  protected editGroupService_Click(): void {
    this.openServicesModal();
  }

  private openServicesModal() {
    let dialogRef = this.dialog.open(OrderGroupServiceUpsertComponent, {
      data: { groupId: this.groupId },
      height: '550px',
      width: '1000px',
      disableClose: true,
      injector: this.injector
    });

    dialogRef.afterClosed().subscribe((isSave: boolean) => {
      if (!isSave) {
        return;
      }
      this.groupServices = this.orderSharedService.getOrderGroupServices(this.groupId);
      this.isGroupHasSellingService = this.orderSharedService.getOrderGroup(this.groupId).isHasSellingService;
      this.updateDisplayedColumns();
    });
  }

  protected getDetailValueByKeyName(itemId: number, key: string) {
    const item: ItemGetDto = this.orderSharedService.getItem(this.groupId, itemId);
    item.itemDetails.find(x => x.key == key)?.value;
  }

  protected addItem_Click() {
    this.navigateToAddItemPage();
  }

  protected editItem_Click(item: ItemGetDto) {
    this.navigateToEditItemPage(item.id);
  }

  protected deleteItem_Click(item: ItemGetDto) {
    // Alert
    // const dialogData: ConfirmDialogModel = {
    //   title: 'تأكيد الحذف',
    //   message: 'هل أنت متأكد أنك تريد حذف هذه الخدمة ؟',
    //   confirmText: 'نعم',
    //   cancelText: 'إلغاء',
    // };

    // const dialogSub = this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
    //   if (confirmed) {
    //     this.orderSharedDataService.deleteGroupService(this.groupId, serviceId);
    //     this.fillPageData();
    //     this.alertService.showSuccess('تم حذف الخدمة بنجاح!');
    //   }
    // });

    // Move to shared service
    if (item.objectState == ObjectStateEnum.temp) {
      this.orderSharedService.deleteNewlyAddedItem(this.groupId, item.id);
    } else {
      this.orderSharedService.deleteExistingItem(this.groupId, item.id);
    }
  }

  onPageChangeClick(event: PageEvent) {
    const length = event.pageSize;
    const pageNumber = event.pageIndex;
    // this.itemsGridSource = this.groupItems.slice((pageNumber) * length, (pageNumber + 1) * length)
  }

  protected onSave_Click() {
    if (!this.validateGroup()) {
      return;
    }

    console.log('Group saved:', {
      groupName: this.groupName,
      groupServices: this.groupServices,
      items: this.groupItems
    });


    if (this.isEdit) {
      this.orderSharedService.updateOrderGroup(this.groupId, this.groupName, this.groupServices);
    } else {
      this.orderSharedService.saveNewOrderGroup(this.groupId, this.groupName, this.groupServices);
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

  protected onBack_Click() {
    // Alert

    if (!this.isEdit) {
      this.orderSharedService.deleteNewlyAddedGroup(this.groupId);
    }

    this.navigateToOrderPage();
  }

  private navigateToOrderPage() {
    this.router.navigate([this.orderSharedService.getOrderPageRoute()]);
  }

  private navigateToAddItemPage() {
    this.router.navigate(['/order/item/add', this.groupId]);
  }

  private navigateToEditItemPage(itemId: number) {
    this.router.navigate(['/order/item/edit', this.groupId, itemId]);
  }

  private mapIntoSellingVM(items: ItemGetDto[]): ItemSellingVM[] {
    let itemSellingVMArr: ItemSellingVM[] = [];
    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      const boughtItemsCount = item.itemDetails.find(x => x.key === itemDetailsKeyEnum.BoughtItemsCount);

      let itemSellingVM: ItemSellingVM = {
        name: item.name,
        quantity: item.quantity,
        price: item.price,
        total: 0,
        boughtItemsCount: boughtItemsCount ? Number(boughtItemsCount.value) : 0
      };
      itemSellingVMArr.push(itemSellingVM);

    }
    return itemSellingVMArr;
  }

  private mapIntoNonSellingVM(items: ItemGetDto[]): ItemNonSellingVM[] {
    let itemSellingVMArr: ItemNonSellingVM[] = [];
    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      const numberOfPages = item.itemDetails.find(x => x.key === itemDetailsKeyEnum.NumberOfPages);
      const printedItemsCount = item.itemDetails.find(x => x.key === itemDetailsKeyEnum.PrintedItemsCount);
      const stapledItemsCount = item.itemDetails.find(x => x.key === itemDetailsKeyEnum.StapledItemsCount);

      let itemSellingVM: ItemNonSellingVM = {
        name: item.name,
        quantity: item.quantity,
        price: item.price,
        total: 0,
        numberOfPages: numberOfPages ? Number(numberOfPages.value) : 0,
        printedItemsCount: printedItemsCount ? Number(printedItemsCount.value) : 0,
        stapledItemsCount: stapledItemsCount ? Number(stapledItemsCount.value) : 0
      };
      itemSellingVMArr.push(itemSellingVM);

    }
    return itemSellingVMArr;
  }
}
