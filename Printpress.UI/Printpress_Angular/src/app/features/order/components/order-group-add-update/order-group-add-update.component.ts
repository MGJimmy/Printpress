import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { OrderGroupUpsertDto } from '../../models/orderGroup/order-group-upsert.Dto';
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

@Component({
  selector: 'app-order-group-add-update',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule, SharedPaginationComponent, CommonModule, MatDialogModule],
  templateUrl: './order-group-add-update.component.html',
  styleUrl: './order-group-add-update.component.css'
})
export class OrderGroupAddUpdateComponent implements OnInit {

  @Input({ required: true }) inputObject!: {};
  @Output() SaveOrderGroup = new EventEmitter<OrderGroupUpsertDto>();

  private groupId!: number | null;
  protected get isEdit(): boolean {
    return this.groupId ? true : false;
  }

  groupName: string = '';
  groupServices: string = '';
  displayedColumns: string[];
  isOuterItem: boolean = false;

  groupItems: { name: string; price: number }[] = [
    { name: 'عنصر 1', price: 30 },
    { name: 'عنصر 2', price: 50 }
  ];

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
    if (this.isEdit) {
      this.orderGroupService.getAllGroupItems().subscribe(items => {

      })
      // get group items from API
    }

    // this.inputObject = this.OrderSharedService.orderObject.orderGroups.filter(x => x.id == this.groupId);
  }

  private setGroupId(): void {
    const param_GroupId = this.route.snapshot.paramMap.get('id');
    this.groupId = param_GroupId ? Number(param_GroupId) : null;
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

  addGroupItem() {
    this.groupItems.push({ name: 'عنصر جديد', price: 0 });
  }

  deleteItem(item: any) {
    const index = this.groupItems.indexOf(item);
    if (index > -1) {
      this.groupItems.splice(index, 1);
    }
  }
  onPageChangeClick(event: PageEvent) {
    console.log(event);
    const length = event.pageSize;
    const pageNumber = event.pageIndex;
    // Call APi and set dataSource
    // this.dataSource = this.originalSource.slice((pageNumber) * length, (pageNumber + 1) * length)
  }
  saveGroup() {
    if (!this.validateGroup()) {
      return;
    }

    console.log('Group saved:', {
      groupName: this.groupName,
      groupServices: this.groupServices,
      items: this.groupItems
    });
  }

  private validateGroup(): boolean {
    if (!this.groupName) {
      this.alertService.showError('أدخل اسم المجموعة');
      return false;
    }

    return true;
  }

  private createFinalObject() {
    // let orderGroupUpsertDto = new OrderGroupUpsertDto();

    // orderGroupUpsertDto.name = '';
    // orderGroupUpsertDto.orderGroupServices = [];
    // orderGroupUpsertDto.items = [];

  }
}
