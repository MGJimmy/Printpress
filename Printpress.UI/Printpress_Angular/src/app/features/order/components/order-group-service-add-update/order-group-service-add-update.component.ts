import { Component, EventEmitter, Output } from '@angular/core';
import { orderGroupServiceAddUpdateDto } from '../../models/orderGroupService/orderGroupServiceAddUpdate.Dto';

@Component({
  selector: 'app-order-group-service-add-update',
  standalone: true,
  imports: [],
  templateUrl: './order-group-service-add-update.component.html',
  styleUrl: './order-group-service-add-update.component.css'
})
export class OrderGroupServiceAddUpdateComponent {
  @Output() SaveOrderGroupService = new EventEmitter<orderGroupServiceAddUpdateDto>();
}
