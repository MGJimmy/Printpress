import { Component, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-order-group-add-update',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule, SharedPaginationComponent, CommonModule, MatDialogModule],
  templateUrl: './order-group-add-update.component.html',
  styleUrl: './order-group-add-update.component.css'
})
export class OrderGroupAddUpdateComponent implements OnInit {

  protected isEdit: boolean = false;

  private groupId!: number;
  protected groupName: string = '';
  // protected groupItems: ItemGetDto[] = [];
  protected groupItems: any[] = [
    { name: 'عنصر 1', price: 30, quantity: 10 },
    { name: 'عنصر 2', price: 50, quantity: 20 }
  ]

  private groupServices: OrderGroupServiceGetDto[] = [];
  protected get displayedGroupServices(): string {
    return this.groupServices.map(x => { return x.serviceName }).join(',');
  }

  protected isOuterItem: boolean = false;
  displayedColumns: string[];

  constructor(private alertService: AlertService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private orderSharedService: OrderSharedDataService
  ) {
    if (!this.isOuterItem) {
      this.displayedColumns = ['index', 'name', 'numberOfPages', 'quantity',
        'itemPrice', 'stapledItemsCount', 'printedItemsCount', 'total', 'actions'];
    } else {
      this.displayedColumns = ['index', 'name', 'quantity',
        'itemPrice', 'boughtItemsCount', 'total', 'actions'];
    }
  }

  ngOnInit(): void {
    this.setGroupId();
    this.setCurrentGroupData();
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

  protected editGroupService_Click(): void {
    let dialogRef = this.dialog.open(OrderGroupServiceUpsertComponent, {
      data: { x: 5 },
      height: '650px',
      width: '1100px',
    });

    dialogRef.afterClosed().subscribe(result => {
      let returnedServices = result as { y: string };
      console.log(returnedServices);
    });
  }

  protected addItem_Click() {
    this.navigateToAddItemPage();
  }

  protected editItem_Click(item: ItemGetDto) {
    this.navigateToEditItemPage(item.id);
  }

  protected deleteItem_Click(item: ItemGetDto) {
    // Alert
    if (item.objectState == ObjectStateEnum.temp) {
      this.orderSharedService.deleteNewlyAddedItem(this.groupId, item.id);
    } else {
      this.orderSharedService.deleteExistingItem(this.groupId, item.id);
    }
  }

  onPageChangeClick(event: PageEvent) {
    console.log(event);
    const length = event.pageSize;
    const pageNumber = event.pageIndex;
    // Call APi and set dataSource
    // this.dataSource = this.originalSource.slice((pageNumber) * length, (pageNumber + 1) * length)
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
    this.router.navigate(['/order/item', this.groupId]);
  }

  private navigateToEditItemPage(itemId: number) {
    this.router.navigate(['/order/item', this.groupId, itemId]);
  }

}
