import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { imports } from './order-add-update.imports';
import { OrderSharedDataService } from '../../services/order-shared-data.service';
import { OrderService } from '../../services/order.service';
import { ClientService } from '../../../client/services/client.service';
import { ClientGetDto } from '../../../client/models/client-get.dto';
import { ComponentMode } from '../../../../shared/models/ComponentMode';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AddClientComponent } from '../../../client/components/add-client/add-client.component';
import { OrderServicePricesComponent } from '../order-service-prices/order-service-prices.component';

@Component({
  selector: 'app-order-add-update',
  standalone: true,
  imports: imports,
  templateUrl: './order-add-update.component.html',
  styleUrl: './order-add-update.component.css',
})
export class OrderAddUpdateComponent implements OnInit {

  public componentMode: ComponentMode;
  public displayedColumns = ['no', 'name', 'deliveryDate', 'action'];
  public dataSource = new MatTableDataSource<OrderGroupGridViewModel>(ELEMENT_DATA);
  public clients: ClientGetDto[] = [];
  public selectedClientId!: number
  public orderName!: string;

  constructor(private router: Router,
    private OrderSharedService: OrderSharedDataService,
    private clientService: ClientService,
    private dialog: MatDialog
  ) {
    this.componentMode = new ComponentMode(this.router);
  }

  ngOnInit(): void {

    if (this.componentMode.isViewMode) {
      this.displayedColumns = this.displayedColumns.filter(col => col !== 'action')
    }

    this.clientService.getAll().subscribe((res) => {

      this.clients = res.data;
    })
  }

  saveOrder_Click() {

    // Validate ////

    // Open Service Prices Component.
    this.dialog.open(OrderServicePricesComponent, {
      data: { orderSharedService: this.OrderSharedService },
      height: '550px',
      width: '1000px'
    });
  }



  openDialog() {
    this.dialog.open(AddClientComponent, {
      width: '600px',
      data: { message: 'Hello from the main component!' }
    });
  }
}

export interface OrderGroupGridViewModel {
  no: number;
  name: string;
  deliveryDate: number;
}
const ELEMENT_DATA: OrderGroupGridViewModel[] = [
  { no: 1, name: 'Hydrogen', deliveryDate: 1.0079 },
  { no: 2, name: 'Helium', deliveryDate: 4.0026 },
  { no: 3, name: 'Lithium', deliveryDate: 6.941 },
];


