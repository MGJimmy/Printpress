import { Component, EventEmitter, Output } from '@angular/core';
import { OrderGroupServiceUpsertDto } from '../../models/orderGroupService/order-group-service-upsert.Dto';

@Component({
  selector: 'app-order-group-service-add-update',
  standalone: true,
  imports: [],
  templateUrl: './order-group-service-add-update.component.html',
  styleUrl: './order-group-service-add-update.component.css'
})
export class OrderGroupServiceAddUpdateComponent {
  @Output() SaveOrderGroupService = new EventEmitter<OrderGroupServiceUpsertDto>();
}
