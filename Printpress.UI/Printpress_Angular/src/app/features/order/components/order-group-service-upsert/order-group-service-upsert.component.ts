import { Component, EventEmitter, Output } from '@angular/core';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.dto';

@Component({
  selector: 'app-order-group-service-add-update',
  standalone: true,
  imports: [],
  templateUrl: './order-group-service-upsert.component.html',
  styleUrl: './order-group-service-upsert.component.css'
})
export class OrderGroupServiceUpsertComponent {
  @Output() SaveOrderGroupService = new EventEmitter<OrderGroupServiceUpsertDto>();
}
