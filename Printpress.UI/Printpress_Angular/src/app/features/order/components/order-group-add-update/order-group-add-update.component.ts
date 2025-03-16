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
import { ItemSharedVM, ItemNonSellingVM, ItemSellingVM } from '../../models/item/itemGridVM';
import { itemDetailsKeyEnum } from '../../models/enums/item-details-key.enum';
import { DialogService } from '../../../../shared/services/dialog.service';

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
  protected groupItems: ItemGetDto[] = [
    {
      id: 1,
      name: "Printer",
      quantity: 5,
      price: 300,
      groupId: 1,
      objectState: ObjectStateEnum.unchanged,
      itemDetails: [
        {
          id: 1,
          itemId: 1,
          key: itemDetailsKeyEnum.NumberOfPages,
          value: "500",
          objectState: ObjectStateEnum.unchanged
        },
        {
          id: 2,
          itemId: 1,
          key: itemDetailsKeyEnum.NumberOfPrintingFaces,
          value: "2",
          objectState: ObjectStateEnum.unchanged
        }
      ]
    },
    {
      id: 2,
      name: "Notebook",
      quantity: 50,
      price: 10,
      groupId: 2,
      objectState: ObjectStateEnum.added,
      itemDetails: [
        {
          id: 3,
          itemId: 2,
          key: itemDetailsKeyEnum.BoughtItemsCount,
          value: "100",
          objectState: ObjectStateEnum.added
        },
        {
          id: 4,
          itemId: 2,
          key: itemDetailsKeyEnum.PrintedItemsCount,
          value: "80",
          objectState: ObjectStateEnum.added
        }
      ]
    },
    {
      id: 3,
      name: "Stapler",
      quantity: 20,
      price: 15,
      groupId: 3,
      objectState: ObjectStateEnum.updated,
      itemDetails: [
        {
          id: 5,
          itemId: 3,
          key: itemDetailsKeyEnum.StapledItemsCount,
          value: "200",
          objectState: ObjectStateEnum.updated
        }
      ]
    },
    {
      id: 1,
      name: "Printer",
      quantity: 5,
      price: 300,
      groupId: 1,
      objectState: ObjectStateEnum.unchanged,
      itemDetails: [
        {
          id: 1,
          itemId: 1,
          key: itemDetailsKeyEnum.NumberOfPages,
          value: "500",
          objectState: ObjectStateEnum.unchanged
        },
        {
          id: 2,
          itemId: 1,
          key: itemDetailsKeyEnum.NumberOfPrintingFaces,
          value: "2",
          objectState: ObjectStateEnum.unchanged
        }
      ]
    },
    {
      id: 2,
      name: "Notebook",
      quantity: 50,
      price: 10,
      groupId: 2,
      objectState: ObjectStateEnum.added,
      itemDetails: [
        {
          id: 3,
          itemId: 2,
          key: itemDetailsKeyEnum.BoughtItemsCount,
          value: "100",
          objectState: ObjectStateEnum.added
        },
        {
          id: 4,
          itemId: 2,
          key: itemDetailsKeyEnum.PrintedItemsCount,
          value: "80",
          objectState: ObjectStateEnum.added
        }
      ]
    },
    {
      id: 3,
      name: "Stapler",
      quantity: 20,
      price: 15,
      groupId: 3,
      objectState: ObjectStateEnum.updated,
      itemDetails: [
        {
          id: 5,
          itemId: 3,
          key: itemDetailsKeyEnum.StapledItemsCount,
          value: "200",
          objectState: ObjectStateEnum.updated
        }
      ]
    },
    {
      id: 1,
      name: "Printer",
      quantity: 5,
      price: 300,
      groupId: 1,
      objectState: ObjectStateEnum.unchanged,
      itemDetails: [
        {
          id: 1,
          itemId: 1,
          key: itemDetailsKeyEnum.NumberOfPages,
          value: "500",
          objectState: ObjectStateEnum.unchanged
        },
        {
          id: 2,
          itemId: 1,
          key: itemDetailsKeyEnum.NumberOfPrintingFaces,
          value: "2",
          objectState: ObjectStateEnum.unchanged
        }
      ]
    },
    {
      id: 2,
      name: "Notebook",
      quantity: 50,
      price: 10,
      groupId: 2,
      objectState: ObjectStateEnum.added,
      itemDetails: [
        {
          id: 3,
          itemId: 2,
          key: itemDetailsKeyEnum.BoughtItemsCount,
          value: "100",
          objectState: ObjectStateEnum.added
        },
        {
          id: 4,
          itemId: 2,
          key: itemDetailsKeyEnum.PrintedItemsCount,
          value: "80",
          objectState: ObjectStateEnum.added
        }
      ]
    },
    {
      id: 3,
      name: "Stapler",
      quantity: 20,
      price: 15,
      groupId: 3,
      objectState: ObjectStateEnum.updated,
      itemDetails: [
        {
          id: 5,
          itemId: 3,
          key: itemDetailsKeyEnum.StapledItemsCount,
          value: "200",
          objectState: ObjectStateEnum.updated
        }
      ]
    }
  ];

  protected itemsGridSource!: ItemSharedVM[];

  protected groupServicesNamesCommaseperated!: string;

  private updateDisplayedServicesNames(groupServices: OrderGroupServiceGetDto[]) {
    this.groupServicesNamesCommaseperated = groupServices.map(x => { return x.serviceName }).join(' - ');
  }

  protected isGroupHasSellingService: boolean = false;

  displayedColumns: string[] = [];

  constructor(private alertService: AlertService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private orderSharedService: OrderSharedDataService,
    private injector: Injector,
    private dialogService: DialogService
  ) {
  }

  ngOnInit(): void {
    this.setGroupId();
    this.orderSharedService.updateGroupFlagsOnServicesCategoriesById(this.groupId);
    this.setCurrentGroupData();
    this.updateDisplayedColumns();

    if (!this.groupItems || this.groupItems.length == 0) {
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
    this.updateDisplayedServicesNames(currentGroup.orderGroupServices);

    this.groupItems = currentGroup.items;
    this.mapItems(this.groupItems);
  }

  private mapItems(items: ItemGetDto[]): void {
    if (this.isGroupHasSellingService) {
      this.itemsGridSource = this.mapIntoSellingVM(items);
    } else {
      this.itemsGridSource = this.mapIntoNonSellingVM(items);
    }
  }

  protected updateDisplayedColumns() {
    if (this.isGroupHasSellingService) {
      this.displayedColumns = ['index', 'name', 'quantity',
        'itemPrice', 'boughtItemsCount', 'total', 'actions'];
    } else {
      this.displayedColumns = ['index', 'name',
        'numberOfPages', 'stapledItemsCount', 'printedItemsCount',
        'quantity', 'itemPrice', 'total', 'actions'];
    }
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
      height: '550px',
      width: '1000px',
      disableClose: true,
      injector: this.injector
    });

    dialogRef.afterClosed().subscribe((isSave: boolean) => {
      if (!isSave) {
        return;
      }
      const groupServices = this.orderSharedService.getOrderGroupServices(this.groupId);
      this.updateDisplayedServicesNames(groupServices);

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
    const dialogData: ConfirmDialogModel = {
      title: 'تأكيد الحذف',
      message: 'هل أنت متأكد أنك تريد حذف هذا العنصر ؟',
      confirmText: 'نعم',
      cancelText: 'إلغاء',
    };

    this.dialogService.confirmDialog(dialogData).subscribe((confirmed) => {
      if (confirmed) {
        this.orderSharedService.deleteItem(this.groupId, item.id);
        this.alertService.showSuccess('تم حذف العنصر بنجاح!');
      }
    });
  }

  protected onPageChangeClick(event: PageEvent) {
    const length = event.pageSize;
    const pageNumber = event.pageIndex;
    const itemsToDisplay = this.groupItems.slice((pageNumber) * length, (pageNumber + 1) * length);
    this.mapItems(itemsToDisplay);
  }

  protected onSave_Click() {
    if (!this.validateGroup()) {
      return;
    }

    if (this.isEdit) {
      this.orderSharedService.updateOrderGroup(this.groupId);
    } else {
      this.orderSharedService.saveNewOrderGroup(this.groupId);
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
        id: item.id,
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
        id: item.id,
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
