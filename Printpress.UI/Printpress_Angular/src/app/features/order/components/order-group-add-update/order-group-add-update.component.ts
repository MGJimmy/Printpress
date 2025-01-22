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
import { OrderGroupService } from "../../services/order-group.service";
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
  protected groupServices: OrderGroupServiceGetDto[] = [];
  protected groupItems: ItemGetDto[] = [];

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
    this.groupItems = currentGroup.items;
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


    this.orderSharedService.updateOrderGroup(this.groupId, this.groupName, this.groupServices, this.groupItems);

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

  private navigateToOrderPage() {
    this.router.navigate(['/order/edit', this.orderSharedService.getOrderObject().id])
  }

  private navigateToAddItemPage() {
    this.router.navigate(['/order/item', this.groupId]);
  }
  private navigateToEditItemPage(itemId: number) {
    this.router.navigate(['/order/item', this.groupId, itemId]);
  }

}
