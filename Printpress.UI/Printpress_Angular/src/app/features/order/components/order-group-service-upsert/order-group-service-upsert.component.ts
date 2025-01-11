import { Component, EventEmitter, Inject, Output } from '@angular/core';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.dto';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-order-group-service-add-update',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './order-group-service-upsert.component.html',
  styleUrl: './order-group-service-upsert.component.css'
})
export class OrderGroupServiceUpsertComponent {

  @Output() SaveOrderGroupService = new EventEmitter<OrderGroupServiceUpsertDto>();

  outputObject: any = { y: 15 };
  constructor(@Inject(MAT_DIALOG_DATA) public data: { x: number }) {

  }
}
