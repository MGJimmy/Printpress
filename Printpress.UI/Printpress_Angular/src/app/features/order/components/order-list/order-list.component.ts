
import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material/table';
import { imports } from './order-list.imports';
import { OrderService } from '../../services/order.service';
import { firstValueFrom } from 'rxjs';
import { OrderSummaryDto } from '../../models/order/order-summary.Dto';
import {DEFAULT_PAGE_NUMBER, DEFAULT_PAGE_SIZE } from '../../../../shared/constatnt/constant';

@Component({
  selector: 'app-order-view',
  standalone: true,
  imports: imports,
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css'
})
export class OrderListComponent implements OnInit {

  public dataSource : MatTableDataSource<OrderSummaryDto>;
  public totalCount:number ;
  public isEditMode:boolean ;
  public displayedColumns : string[];

  constructor(private orderService:OrderService)  {
    this.dataSource = new MatTableDataSource<OrderSummaryDto>();
    this.totalCount = 0;
    this.isEditMode = false;
    this.displayedColumns = ['orderName', 'clientName', 'totalAmount', 'paidAmount','orderStatus' ,'createdAt', 'action' ];
  }

 async ngOnInit(){
  
  const response = await firstValueFrom(this.orderService.getOrdersSummaryList(DEFAULT_PAGE_SIZE,DEFAULT_PAGE_NUMBER));

  this.dataSource.data = response.data.items;
  this.totalCount = response.data.totalCount;

  }


}

