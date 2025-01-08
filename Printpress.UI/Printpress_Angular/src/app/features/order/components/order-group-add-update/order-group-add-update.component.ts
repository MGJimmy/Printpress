import { Component, EventEmitter, Output } from '@angular/core';
import { OrderGroupUpsertDto } from '../../models/orderGroup/order-group-upsert.Dto';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {  MatTableModule } from '@angular/material/table';
import {  MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PageEvent } from '@angular/material/paginator';
import { SharedPaginationComponent } from '../../../../shared/components/shared-pagination/shared-pagination.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-order-group-add-update',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, MatButtonModule, MatIconModule, MatTableModule, SharedPaginationComponent, CommonModule],
  templateUrl: './order-group-add-update.component.html',
  styleUrl: './order-group-add-update.component.css'
})
export class OrderGroupAddUpdateComponent {
  @Output() SaveOrderGroup = new EventEmitter<OrderGroupUpsertDto>();
  groupName: string = '';
  groupServices: string = '';
  displayedColumns: string[];
  isOuterItem: boolean = true;

  dataSource: { name: string; price: number }[] = [
    { name: 'عنصر 1', price: 30 },
    { name: 'عنصر 2', price: 50 }
  ];
  /**
   *
   */
  constructor() {
    if (!this.isOuterItem) {
      this.displayedColumns = ['index', 'name', 'numberOfPages', 'quantity',
        'itemPrice', 'stapledItemsCount', 'printedItemsCount', 'total', 'actions'];
    } else {
      this.displayedColumns = ['index', 'name', 'quantity',
        'itemPrice', 'boughtItemsCount', 'total', 'actions'];
    }
  }

  addGroupItem() {
    this.dataSource.push({ name: 'عنصر جديد', price: 0 });
  }

  deleteItem(item: any) {
    const index = this.dataSource.indexOf(item);
    if (index > -1) {
      this.dataSource.splice(index, 1);
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
    console.log('Group saved:', {
      groupName: this.groupName,
      groupServices: this.groupServices,
      items: this.dataSource
    });
  }
}
