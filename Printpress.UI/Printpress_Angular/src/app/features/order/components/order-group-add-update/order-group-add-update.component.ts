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

@Component({
  selector: 'app-order-group-add-update',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule, SharedPaginationComponent, CommonModule, MatDialogModule],
  templateUrl: './order-group-add-update.component.html',
  styleUrl: './order-group-add-update.component.css'
})
export class OrderGroupAddUpdateComponent implements OnInit {

  protected isEdit: boolean = false; // shall be removed.

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
    private orderGroupService: OrderGroupService,
    private OrderSharedService: OrderSharedDataService
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
    this.groupId = Number(param_GroupId);
  }

  private setCurrentGroupData() {
    // should be intialized from order Page
    // const id = this.OrderSharedService.intializeNewGroup();
    // const currentGroup = this.OrderSharedService.getOrderGroup(id);

    const currentGroup = this.OrderSharedService.getOrderGroup(this.groupId);

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
    // const newItemId = this.OrderSharedService.intializeNewGroupItem(this.groupId);
    // this.navigateToAddItemPage(newItemId);
  }

  protected editItem_Click(item: ItemGetDto) {
    this.navigateToAddItemPage(item.id);
  }

  protected deleteItem_Click(item: ItemGetDto) {
    let groupItems = this.OrderSharedService.getOrderGroup(this.groupId).items;

    const index = groupItems.findIndex(x => x.id === item.id);
    if (index !== -1) {
      groupItems.splice(index, 1);
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


    this.OrderSharedService.updateOrderGroup(this.groupId, this.groupName, this.groupServices, this.groupItems);

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
    this.router.navigate(['/order/edit', this.OrderSharedService.getOrderObject().id])
  }

  private navigateToAddItemPage(itemId: number) {
    this.router.navigate(['/order/item', this.groupId, itemId]);
  }
}
